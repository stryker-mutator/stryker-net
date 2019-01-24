using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.TestRunners;
using System.Diagnostics;
using Stryker.Core.Coverage;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        int InitialTest(ITestRunner testRunner);
    }
    
    public class InitialTestProcess : IInitialTestProcess
    {
        private ILogger _logger { get; set; }
        private string coverageReport;

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

            using (var coverageServer = new CoverageServer("Coverage"))
            {
                coverageServer.RaiseReceivedMessage += CoverageServer_RaiseReceivedMessage;
                coverageServer.Listen();
                // setup a stopwatch to record the initial test duration
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var testResult = testRunner.CaptureCoverage(coverageServer.PipeName);
                var duration = (int)stopwatch.ElapsedMilliseconds;
                _logger.LogInformation("Total number of tests found in initial test run: {0}", testResult.TotalNumberOfTests);


                _logger.LogDebug("Initial testrun output {0}", testResult.ResultMessage);
                if (!testResult.Success)
                {
                    throw new StrykerInputException("Initial testrun was not successful.", testResult.ResultMessage);
                }
                _logger.LogInformation("Initial testrun successful in {0} ms", duration);

                return duration;
            }
        }

        private void CoverageServer_RaiseReceivedMessage(object sender, string args)
        {
            coverageReport = args;
        }
    }
}
