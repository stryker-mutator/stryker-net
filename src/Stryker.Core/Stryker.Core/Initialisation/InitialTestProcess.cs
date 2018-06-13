using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.TestRunners;
using System;
using System.Diagnostics;

namespace Stryker.Core.Initialisation
{
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

            var testResult = testRunner.RunAll();

            var duration = (int)stopwatch.ElapsedMilliseconds;

            _logger.LogDebug("Initial testrun output {0}", testResult.ResultMessage);
            if (!testResult.Success)
            {
                throw new InitialTestRunFailedException("The initial testrun was not successful. Please review your tests.", new Exception(testResult.ResultMessage));
            }
            _logger.LogInformation("Initial testrun successful in {0} ms", duration);

            return duration;
        }
    }
}
