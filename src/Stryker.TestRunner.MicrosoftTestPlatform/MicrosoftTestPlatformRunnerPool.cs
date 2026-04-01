using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
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
    private readonly SemaphoreSlim _runnerAvailable;
    private readonly ConcurrentBag<SingleMicrosoftTestPlatformRunner> _availableRunners = new();
    private readonly ILogger _logger;
    private readonly int _countOfRunners;
    private readonly TestSet _testSet = new();
    private readonly Dictionary<string, List<TestNode>> _testsByAssembly = new();
    private readonly Dictionary<string, MtpTestDescription> _testDescriptions = new();
    private readonly object _discoveryLock = new();
    private readonly ISingleRunnerFactory _runnerFactory;
    private readonly IStrykerOptions _options;

    public IEnumerable<SingleMicrosoftTestPlatformRunner> Runners => _availableRunners;

    public MicrosoftTestPlatformRunnerPool(IStrykerOptions options, ILogger? logger = null, ISingleRunnerFactory? runnerFactory = null)
    {
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<MicrosoftTestPlatformRunnerPool>();
        _options = options;
        _countOfRunners = Math.Max(1, options.Concurrency);
        _runnerFactory = runnerFactory ?? new DefaultRunnerFactory();
        _runnerAvailable = new SemaphoreSlim(0, _countOfRunners);
        _logger.LogWarning("The Microsoft Test Platform testrunner is currently in preview. Results should be verified since this feature is still being tested.");

        Initialize();
    }

    public void ResetTestProcesses()
    {
        _logger.LogDebug("Resetting all test server processes in the pool");
        var tasks = _availableRunners.Select(runner => runner.ResetServerAsync());
        Task.WhenAll(tasks).Wait();
        _logger.LogDebug("All test server processes have been reset");
    }

    private void Initialize()
    {
        // Create and initialize all runners in parallel to speed up startup time
        Parallel.For(0, _countOfRunners, (int i, ParallelLoopState _) =>
        {
            var runner = _runnerFactory.CreateRunner(
                i,
                _testsByAssembly,
                _testDescriptions,
                _testSet,
                _discoveryLock,
                _logger,
                _options);
            _availableRunners.Add(runner);
            _runnerAvailable.Release();
        });
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

        var results = await RunThisAsync(runner => runner.InitialTestAsync(project)).ConfigureAwait(false);

        // reset all test processes after the initial test run
        ResetTestProcesses();

        return results;
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        if (_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
        {
            var confidence = _options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest)
                ? CoverageConfidence.Exact
                : CoverageConfidence.Normal;
            return CaptureCoverageTestByTest(project, confidence);
        }

        return CaptureCoverageInOneGo(project);
    }

    private IEnumerable<ICoverageRunResult> CaptureCoverageInOneGo(IProjectAndTests project)
    {
        _logger.LogInformation("Starting aggregate coverage capture for MTP runner");

        foreach (var runner in _availableRunners)
        {
            runner.SetCoverageMode(true);
        }

        try
        {
            var testResult = RunThisAsync(runner => runner.InitialTestAsync(project)).GetAwaiter().GetResult();

            if (testResult.FailingTests.IsEveryTest)
            {
                _logger.LogWarning("Coverage test run failed: {Message}", testResult.ResultMessage);
            }

            ResetTestProcesses();

            var allCoveredMutants = new HashSet<int>();
            var allStaticMutants = new HashSet<int>();

            foreach (var runner in _availableRunners)
            {
                var (coveredMutants, staticMutants) = runner.ReadCoverageData();
                foreach (var mutantId in coveredMutants)
                {
                    allCoveredMutants.Add(mutantId);
                }
                foreach (var mutantId in staticMutants)
                {
                    allStaticMutants.Add(mutantId);
                }
            }

            _logger.LogInformation("Aggregate coverage capture complete: {CoveredCount} mutations covered, {StaticCount} static mutations",
                allCoveredMutants.Count, allStaticMutants.Count);

            return _testDescriptions.Values.Select(testDescription =>
                CoverageRunResult.Create(
                    testDescription.Id,
                    CoverageConfidence.Normal,
                    allCoveredMutants,
                    allStaticMutants,
                    []));
        }
        finally
        {
            foreach (var runner in _availableRunners)
            {
                runner.SetCoverageMode(false);
            }
        }
    }

    private IEnumerable<ICoverageRunResult> CaptureCoverageTestByTest(
        IProjectAndTests project, CoverageConfidence confidence)
    {
        _logger.LogInformation("Starting per-test coverage capture for MTP runner");

        foreach (var runner in _availableRunners)
        {
            runner.SetCoverageMode(true);
        }

        try
        {
            var allTests = new List<(string Assembly, TestNode Test, string TestId)>();
            foreach (var (assembly, tests) in _testsByAssembly)
            {
                foreach (var test in tests)
                {
                    if (_testDescriptions.TryGetValue(test.Uid, out var desc))
                    {
                        allTests.Add((assembly, test, desc.Id));
                    }
                }
            }

            _logger.LogInformation("Capturing per-test coverage for {TestCount} tests across {AssemblyCount} assemblies",
                allTests.Count, _testsByAssembly.Count);

            var results = new ConcurrentBag<ICoverageRunResult>();

            Parallel.ForEach(allTests,
                new ParallelOptions { MaxDegreeOfParallelism = _countOfRunners },
                testInfo =>
                {
                    var result = RunThisAsync(async runner =>
                        await runner.RunSingleTestForCoverageAsync(
                            testInfo.Assembly, testInfo.Test, testInfo.TestId, confidence)
                            .ConfigureAwait(false))
                        .GetAwaiter().GetResult();

                    results.Add(result);
                });

            _logger.LogInformation(
                "Per-test coverage capture complete: {TestCount} tests captured",
                results.Count);

            return results;
        }
        finally
        {
            foreach (var runner in _availableRunners)
            {
                runner.SetCoverageMode(false);
            }
        }
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
        const int maxWaitTimeSeconds = 300;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(maxWaitTimeSeconds));

        try
        {
            await _runnerAvailable.WaitAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Timed out waiting for an available test runner after {maxWaitTimeSeconds} seconds. Available runners: {_availableRunners.Count}, Total runners: {_countOfRunners}");
        }

        if (!_availableRunners.TryTake(out var runner))
        {
            // Another thread grabbed the runner between the semaphore release and our TryTake; re-wait
            _runnerAvailable.Release();
            return await RunThisAsync(task).ConfigureAwait(false);
        }

        try
        {
            return await task(runner).ConfigureAwait(false);
        }
        finally
        {
            _availableRunners.Add(runner);
            _runnerAvailable.Release();
        }
    }

    public void Dispose()
    {
        foreach (var runner in _availableRunners)
        {
            runner.Dispose();
        }
        _runnerAvailable.Dispose();
    }
}

