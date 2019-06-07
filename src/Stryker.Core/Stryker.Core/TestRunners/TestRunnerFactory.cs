using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunnerFactory
    {
        private ILogger _logger { get; set; }

        public TestRunnerFactory()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<TestRunnerFactory>();
        }

        public ITestRunner Create(StrykerOptions options, ProjectInfo projectInfo)
        {
            _logger.LogDebug("Factory is creating testrunner for asked type {0}", options.TestRunner);
            ITestRunner testRunner;

            switch (options.TestRunner)
            {
                case TestRunner.DotnetTest:
                    testRunner = new DotnetTestRunner(options.BasePath, new ProcessExecutor());
                    break;
                case TestRunner.VsTest:
                    testRunner = new VsTestRunnerPool(options, projectInfo);
                    break;
                default:
                    testRunner = new DotnetTestRunner(options.BasePath, new ProcessExecutor());
                    break;
            }
            _logger.LogInformation("Using testrunner {0}", options.TestRunner);
            return testRunner;
        }
    }
}
