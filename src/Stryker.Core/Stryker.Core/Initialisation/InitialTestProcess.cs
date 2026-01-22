using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.Initialisation;

public interface IInitialTestProcess
{
    InitialTestRun InitialTest(IStrykerOptions options, IProjectAndTests project, ITestRunner testRunner);
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
    public InitialTestRun InitialTest(IStrykerOptions options, IProjectAndTests project, ITestRunner testRunner)
    {
        // Setup a stopwatch to record the initial test duration
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var initTestRunResultTask = testRunner.InitialTestAsync(project);
        var initTestRunResult = initTestRunResultTask.GetAwaiter().GetResult();
        // Stop stopwatch immediately after test run
        stopwatch.Stop();

        // timings
        _logger.LogDebug("Initial test run output: {ResultMessage}.", initTestRunResult.ResultMessage);

        TimeoutValueCalculator = new TimeoutValueCalculator(options.AdditionalTimeout,
            (int)stopwatch.ElapsedMilliseconds,
            (int)initTestRunResult.Duration.TotalMilliseconds);

        return new InitialTestRun(initTestRunResult, TimeoutValueCalculator);
    }
}
