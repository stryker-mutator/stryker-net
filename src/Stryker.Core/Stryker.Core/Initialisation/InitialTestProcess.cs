using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using System;

namespace Stryker.Core.Initialisation
{
    public class InitialTestProcess : IInitialTestProcess
    {
        private IProcessExecutor _processExecutor { get; set; }
        private ILogger _logger { get; set; }

        public InitialTestProcess(IProcessExecutor processExecutor = null)
        {
            _processExecutor = processExecutor ?? new ProcessExecutor();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialTestProcess>();
        }

        public void InitialTest(ITestRunner testRunner)
        {
            _logger.LogInformation("Initial testrun started");
            var testResult = testRunner.RunAll();
            _logger.LogDebug("Initial testrun output {0}", testResult.ResultMessage);
            if (!testResult.Success)
            {
                throw new InitialTestRunFailedException("The initial testrun was not successful. Please review your tests.", new Exception(testResult.ResultMessage));
            }
            _logger.LogInformation("Initial testrun successful");
        }
    }
}
