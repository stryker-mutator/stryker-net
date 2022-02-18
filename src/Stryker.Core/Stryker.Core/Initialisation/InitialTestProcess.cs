using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        InitialTestRun InitialTest(StrykerOptions options, ITestRunner testRunner);
    }

    public class InitialTestProcess : IInitialTestProcess
    {
        private readonly ILogger _logger;

        public InitialTestProcess()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialTestProcess>();
        }

        public ITimeoutValueCalculator TimeoutValueCalculator { get; private set; }

        /// <summary>
        /// Executes the initial testrun using the given testrunner
        /// </summary>
        /// <param name="testRunner"></param>
        /// <param name="options">Stryker options</param>
        /// <returns>The duration of the initial testrun</returns>
        public InitialTestRun InitialTest(StrykerOptions options, ITestRunner testRunner)
        {
            var message = testRunner.DiscoverTests() is var total && total.Count == 0 ? "Unable to detect" : total.Count.ToString();

            _logger.LogInformation("Total number of tests found: {0}.", message);

            _logger.LogInformation("Initial testrun started.");

            // Setup a stopwatch to record the initial test duration
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var initTestRunResult = testRunner.InitialTest();
            // Stop stopwatch immediately after testrun
            stopwatch.Stop();

            // timings
            _logger.LogDebug("Initial testrun output: {0}.", initTestRunResult.ResultMessage);
            if (!initTestRunResult.FailingTests.IsEmpty)
            {
                var failingTestsCount = initTestRunResult.FailingTests.Count;
                _logger.LogWarning($"{(failingTestsCount == 1 ? "A test is ": $"{failingTestsCount} tests are")} failing. Stryker will continue but outcome will be impacted.");
                if (((double)failingTestsCount) / initTestRunResult.RanTests.Count >= .5)
                {
                    throw new InputException("Initial testrun has more than 50% failing tests.", initTestRunResult.ResultMessage);
                }
            }

            TimeoutValueCalculator = new TimeoutValueCalculator(options.AdditionalTimeout,
                (int)stopwatch.ElapsedMilliseconds - (int)initTestRunResult.Duration.TotalMilliseconds,
                (int)stopwatch.ElapsedMilliseconds);

            return new InitialTestRun(initTestRunResult, TimeoutValueCalculator);
        }
    }
}
