using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners.VsTest;
using System.Linq;

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
                case TestRunner.DotnetTest:
                default:
                    testRunner = new DotnetTestRunner(projectInfo.ProjectUnderTestAnalyzerResult.ProjectFilePath, new ProcessExecutor(), options.Optimizations, projectInfo.TestProjectAnalyzerResults.Select(x => x.GetAssemblyPath()));
                    break;
                case TestRunner.VsTest:
                    testRunner = new VsTestRunnerPool(options, projectInfo);
                    break;
            }
            return testRunner;
        }
    }
}
