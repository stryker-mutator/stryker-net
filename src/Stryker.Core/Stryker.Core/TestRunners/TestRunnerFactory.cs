using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.TestRunners
{
    public class TestRunnerFactory
    {
        private ILogger _logger { get; set; }

        public TestRunnerFactory()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<TestRunnerFactory>();
        }

        public ITestRunner Create(string type, string path)
        {
            _logger.LogDebug("Factory is creating testrunner for asked type {0}", type);
            ITestRunner testRunner = null;
            switch (type)
            {
                case("dotnet test"):
                    testRunner = new DotnetTestRunner(path);
                    break;
                default:
                    testRunner = new DotnetTestRunner(path);
                    break;
            }
            _logger.LogInformation("Using testrunner {0}", testRunner.GetType().Name);
            return testRunner;
        }
    }
}
