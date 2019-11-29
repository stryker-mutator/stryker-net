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
        private readonly ILogger _logger;

        public TestRunnerFactory()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<TestRunnerFactory>();
        }

        public ITestRunner Create(StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo)
        {
            _logger.LogInformation("Initializing test runners ({0})", options.TestRunner);
            ITestRunner testRunner;

            switch (options.TestRunner)
            {
                case TestRunner.DotnetTest:
                default:
                    testRunner = new DotnetTestRunner(projectInfo.TestProjectAnalyzerResult.ProjectFilePath, new ProcessExecutor(), flags);
                    break;
                case TestRunner.VsTest:
                    testRunner = new VsTestRunnerPool(options, flags, projectInfo);
                    break;
            }
            _logger.LogInformation("Test runners are ready", options.TestRunner.ToString());
            return testRunner;
        }
    }
}
