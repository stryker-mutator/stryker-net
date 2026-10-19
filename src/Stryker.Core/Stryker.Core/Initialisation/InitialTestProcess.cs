using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.Initialisation;

public interface IInitialTestProcess
{
    Task<InitialTestRun> InitialTestAsync(IStrykerOptions options, IProjectAndTests project, ITestRunner testRunner);
}

public class InitialTestProcess : IInitialTestProcess
{
    private readonly ILogger _logger;

    public InitialTestProcess(ILogger<InitialTestProcess> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ITimeoutValueCalculator TimeoutValueCalculator { get; private set; }

    /// <summary>
    /// Executes the initial test run using the given testrunner
    /// </summary>
    /// <param name="project"></param>
    /// <param name="testRunner"></param>
    /// <param name="options">Stryker options</param>
    /// <returns>The duration of the initial test run</returns>
    public async Task<InitialTestRun> InitialTestAsync(IStrykerOptions options, IProjectAndTests project, ITestRunner testRunner)
    {
        // Setup a stopwatch to record the initial test duration
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var initTestRunResult = await testRunner.InitialTestAsync(project);
        // Stop stopwatch immediately after test run
        stopwatch.Stop();

        // timings
        _logger.LogDebug("Initial test run output: {ResultMessage}.", initTestRunResult.ResultMessage);

        var initialRunWallClockMs = (int)stopwatch.ElapsedMilliseconds;
        var aggregatedTestExecutionMs = (int)initTestRunResult.Duration.TotalMilliseconds;

        TimeoutValueCalculator = new TimeoutValueCalculator(options.AdditionalTimeout,
            initialRunWallClockMs,
            aggregatedTestExecutionMs,
            options.TimeoutRatio);

        _logger.LogInformation(
            "Stryker mutant per-test timeout baseline is {DefaultTimeoutMs} ms (timeout ratio {TimeoutRatio}, additional timeout {AdditionalTimeoutMs} ms, initial run wall clock {InitialRunWallClockMs} ms, aggregated test execution {AggregatedTestExecutionMs} ms).",
            TimeoutValueCalculator.DefaultTimeout,
            options.TimeoutRatio,
            options.AdditionalTimeout,
            initialRunWallClockMs,
            aggregatedTestExecutionMs);

        return new InitialTestRun(initTestRunResult, TimeoutValueCalculator);
    }
}
