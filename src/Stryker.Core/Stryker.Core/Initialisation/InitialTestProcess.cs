using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.TestRunners;
using System.Diagnostics;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        int InitialTest(ITestRunner testRunner);
    }

    public class InitialTestProcess : IInitialTestProcess
    {
        private ILogger _logger { get; set; }

        public InitialTestProcess()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialTestProcess>();
        }

        /// <summary>
        /// Executes the initial testrun using the given testrunner
        /// </summary>
        /// <param name="testRunner"></param>
        /// <returns>The duration of the initial testrun</returns>
        public int InitialTest(ITestRunner testRunner)
        {
            _logger.LogInformation("Initial testrun started");
            // setup a stopwatch to record the initial test duration
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var testResult = testRunner.CaptureCoverage();
            var duration = (int) stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Total number of tests found in initial test run: {0}",
                testResult.TotalNumberOfTests);

            _logger.LogDebug("Initial testrun output {0}", testResult.ResultMessage);
            if (!testResult.Success)
            {
                throw new StrykerInputException("Initial testrun was not successful.", testResult.ResultMessage);
            }

            _logger.LogInformation("Initial testrun successful in {0} ms", duration);

            return duration;
        }
    }
}
