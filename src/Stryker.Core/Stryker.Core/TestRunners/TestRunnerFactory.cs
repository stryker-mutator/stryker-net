using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
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

        public ITestRunner Create(StrykerOptions options, OptimizationModes flags, ProjectInfo projectInfo)
        {
            ITestRunner testRunner = new VsTestRunnerPool(options, flags, projectInfo);
            _logger.LogInformation("Test runners are ready");
            return testRunner;
        }
    }
}
