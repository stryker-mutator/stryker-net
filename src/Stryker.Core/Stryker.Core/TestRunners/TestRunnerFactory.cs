using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Parsers;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners.VsTest;
using System.Runtime.InteropServices;

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

            // TODO: Make vstest work for dotnet core on non-windows systems.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && options.TestRunner != TestRunner.DotnetTest)
            {
                Logger.LogWarning("Using testrunner {0} because OS is not windows. Stryker does not currently support vstest on non-windows OS.", TestRunner.DotnetTest.ToString());
                return new DotnetTestRunner(projectInfo.TestProjectPath, new ProcessExecutor(), new TotalNumberOfTestsParser());
            }

            switch (options.TestRunner)
            {
                case TestRunner.DotnetTest:
                    testRunner = new DotnetTestRunner(projectInfo.TestProjectPath, new ProcessExecutor(), new TotalNumberOfTestsParser());
                    break;
                case TestRunner.VsTest:
                    testRunner = new VsTestRunnerPool(options, flags, projectInfo);
                    break;
                default:
                    testRunner = new DotnetTestRunner(projectInfo.TestProjectPath, new ProcessExecutor(), new TotalNumberOfTestsParser());
                    break;
            }
            Logger.LogInformation("Using testrunner {0}", options.TestRunner.ToString());
            return testRunner;
        }
    }
}
