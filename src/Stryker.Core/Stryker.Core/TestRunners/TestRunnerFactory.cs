using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Parsers;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunnerFactory
    {
        private ILogger Logger { get; }

        public TestRunnerFactory()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<TestRunnerFactory>();
        }

        public ITestRunner Create(StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo)
        {
            Logger.LogDebug("Factory is creating testrunner for asked type {0}", options.TestRunner);
            ITestRunner testRunner = null;

            switch (options.TestRunner)
            {
                case TestRunner.DotnetTest:
                default:
                    testRunner = new DotnetTestRunner(options.BasePath, new ProcessExecutor(), new TotalNumberOfTestsParser(), flags);
                    break;
                case TestRunner.VsTest:
                    testRunner = new VsTestRunnerPool(options, flags, projectInfo);
                    break;
            }
            Logger.LogInformation("Using testrunner {0}", options.TestRunner.ToString());
            return testRunner;
        }
    }
}
