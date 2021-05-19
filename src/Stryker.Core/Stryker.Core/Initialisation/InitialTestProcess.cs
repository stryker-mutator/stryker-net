using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.TestRunners;
using System.Diagnostics;
using Stryker.Core.Options;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        ITimeoutValueCalculator InitialTest(IStrykerOptions options, ITestRunner testRunner);
        TestRunResult InitialTestRun { get; }
    }

    public class InitialTestProcess : IInitialTestProcess
    {
        private readonly ILogger _logger;
        private TestRunResult _initTestRunResult;

        public InitialTestProcess()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialTestProcess>();
        }

        public TestRunResult InitialTestRun => _initTestRunResult;

        /// <summary>
        /// Executes the initial testrun using the given testrunner
        /// </summary>
        /// <param name="testRunner"></param>
        /// <param name="options">Stryker options</param>
        /// <returns>The duration of the initial testrun</returns>
        public ITimeoutValueCalculator InitialTest(IStrykerOptions options, ITestRunner testRunner)
        {
            var message = testRunner.DiscoverTests() is var total && total.Count == 0 ? "Unable to detect" : $"{total}";

            _logger.LogInformation("Total number of tests found: {0}.", message);

            _logger.LogInformation("Initial testrun started.");

            // Setup a stopwatch to record the initial test duration
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _initTestRunResult = testRunner.InitialTest();
            // Stop stopwatch immediately after testrun
            stopwatch.Stop();

            // timings
            _logger.LogDebug("Initial testrun output: {0}.", _initTestRunResult.ResultMessage);
            if (!_initTestRunResult.FailingTests.IsEmpty)
            {
                var failingTestsCount = _initTestRunResult.FailingTests.Count;
                _logger.LogWarning($"{failingTestsCount} tests are failing.");
                if (((double)failingTestsCount) / _initTestRunResult.RanTests.Count > .1)
                {
                    throw new StrykerInputException("Initial testrun has more han 10% failing tests.", _initTestRunResult.ResultMessage);
                }
            }

            return new TimeoutValueCalculator(options.AdditionalTimeoutMS,
                (int)stopwatch.ElapsedMilliseconds-(int)_initTestRunResult.Duration.TotalMilliseconds ,
                (int)stopwatch.ElapsedMilliseconds);
        }
    }
}
