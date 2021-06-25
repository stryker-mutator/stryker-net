using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunnerFactory
    {
        private readonly ILogger _logger;

        public TestRunnerFactory()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<TestRunnerFactory>();
        }

        public ITestRunner Create(IStrykerOptions options, ProjectInfo projectInfo)
        {
            _logger.LogInformation("Initializing test runners ({0})", options.TestRunner);
            ITestRunner testRunner;

            switch (options.TestRunner)
            {
                default:
                case TestRunner.VsTest:
                    testRunner = new VsTestRunnerPool(options, projectInfo);
                    break;
            }
            return testRunner;
        }
    }
}
