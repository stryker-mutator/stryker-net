using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.Utilities.Logging;
using static Stryker.Abstractions.Testing.ITestRunner;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Manages a pool of MicrosoftTestPlatformRunner instances to enable parallel mutation testing
/// with isolated environment variables per runner.
/// </summary>
public sealed class MicrosoftTestPlatformRunnerPool : ITestRunner
{
    private readonly AutoResetEvent _runnerAvailableHandler = new(false);
    private readonly ConcurrentBag<SingleMicrosoftTestPlatformRunner> _availableRunners = new();
    private readonly ILogger _logger;
    private readonly int _countOfRunners;
    private readonly TestSet _testSet = new();
    private readonly Dictionary<string, List<MsTestRunnerDemo.Models.TestNode>> _testsByAssembly = new();
    private readonly Dictionary<string, MtpTestDescription> _testDescriptions = new();
    private readonly object _discoveryLock = new();

    public MicrosoftTestPlatformRunnerPool(IStrykerOptions options)
    {
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<MicrosoftTestPlatformRunnerPool>();
        _countOfRunners = Math.Max(1, options.Concurrency);
        Initialize();
    }

    internal MicrosoftTestPlatformRunnerPool(int concurrency, ILogger? logger = null)
    {
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<MicrosoftTestPlatformRunnerPool>();
        _countOfRunners = Math.Max(1, concurrency);
        Initialize();
    }

    private void Initialize()
    {
        _ = Task.Run(() =>
            Parallel.For(0, _countOfRunners, (int i, ParallelLoopState _) =>
            {
                var runner = new SingleMicrosoftTestPlatformRunner(
                    i,
                    _testsByAssembly,
                    _testDescriptions,
                    _testSet,
                    _discoveryLock,
                    _logger);
                _availableRunners.Add(runner);
                _runnerAvailableHandler.Set();
            }));
    }

    public async Task<bool> DiscoverTestsAsync(string assembly)
    {
        if (string.IsNullOrEmpty(assembly) || !File.Exists(assembly))
        {
            return false;
        }

        return await RunThisAsync(runner => runner.DiscoverTestsAsync(assembly)).ConfigureAwait(false);
    }

    public ITestSet GetTests(IProjectAndTests project) => _testSet;

    public async Task<ITestRunResult> InitialTestAsync(IProjectAndTests project)
    {
        var assemblies = project.GetTestAssemblies();
        if (!assemblies.Any())
        {
            return new TestRunResult(false, "No test assemblies found");
        }

        return await RunThisAsync(runner => runner.InitialTestAsync(project)).ConfigureAwait(false);
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        var coverageResults = new List<ICoverageRunResult>();

        foreach (var testDescription in _testDescriptions.Values)
        {
            var coverageResult = CoverageRunResult.Create(
                testDescription.Id,
                CoverageConfidence.Dubious,
                [],
                [],
                []);

            coverageResults.Add(coverageResult);
        }

        return coverageResults;
    }

    public async Task<ITestRunResult> TestMultipleMutantsAsync(
        IProjectAndTests project,
        ITimeoutValueCalculator? timeoutCalc,
        IReadOnlyList<IMutant> mutants,
        TestUpdateHandler? update)
    {
        var assemblies = project.GetTestAssemblies();
        if (!assemblies.Any())
        {
            return new TestRunResult(false, "No test assemblies found");
        }

        return await RunThisAsync(runner => runner.TestMultipleMutantsAsync(project, timeoutCalc, mutants, update)).ConfigureAwait(false);
    }

    private async Task<T> RunThisAsync<T>(Func<SingleMicrosoftTestPlatformRunner, Task<T>> task)
    {
        SingleMicrosoftTestPlatformRunner? runner;

        // Try to get a runner with a timeout to prevent indefinite blocking
        var attempts = 0;
        const int maxWaitTimeSeconds = 300; // 5 minutes max wait
        const int waitIntervalMs = 1000; // Check every second
        var maxAttempts = maxWaitTimeSeconds * 1000 / waitIntervalMs;

        while (!_availableRunners.TryTake(out runner))
        {
            if (!_runnerAvailableHandler.WaitOne(waitIntervalMs))
            {
                attempts++;
                if (attempts >= maxAttempts)
                {
                    throw new TimeoutException($"Timed out waiting for an available test runner after {maxWaitTimeSeconds} seconds. Available runners: {_availableRunners.Count}, Total runners: {_countOfRunners}");
                }

                if (attempts % 30 == 0) // Log every 30 seconds
                {
                    _logger.LogWarning("Waiting for available test runner... ({Attempts}s elapsed, {Available}/{Total} runners available)",
                        attempts, _availableRunners.Count, _countOfRunners);
                }
            }
        }

        try
        {
            return await task(runner).ConfigureAwait(false);
        }
        finally
        {
            _availableRunners.Add(runner);
            _runnerAvailableHandler.Set();
        }
    }

    public void Dispose()
    {
        foreach (var runner in _availableRunners)
        {
            runner.Dispose();
        }
        _runnerAvailableHandler.Dispose();
    }
}

