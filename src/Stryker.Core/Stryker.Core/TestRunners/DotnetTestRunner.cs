using Stryker.Core.Parsers;
using Stryker.Core.Testing;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private readonly ITotalNumberOfTestsParser _totalNumberOfTestsParser;
        private string _path { get; set; }
        private IProcessExecutor _processExecutor { get; set; }
        private static ILogger _logger { get; set; }

        static DotnetTestRunner()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();

        }

        public DotnetTestRunner(string path, IProcessExecutor processProxy, ITotalNumberOfTestsParser totalNumberOfTestsParser)
        {
            _totalNumberOfTestsParser = totalNumberOfTestsParser;
            _path = path;
            _processExecutor = processProxy;
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            Dictionary<string, string> envVars = activeMutationId == null ? null : 
                new Dictionary<string, string>
            {
                {"ActiveMutation", activeMutationId.ToString() }
            };
            return LaunchTestProcess(timeoutMS, envVars);
        }

        private TestRunResult LaunchTestProcess(int? timeoutMS, Dictionary<string, string> envVars)
        {
            var result = _processExecutor.Start(
                _path,
                "dotnet",
                "test --no-build --no-restore",
                envVars,
                timeoutMS ?? 0);

            return new TestRunResult
            {
                Success = result.ExitCode == 0,
                ResultMessage = result.Output,
                TotalNumberOfTests = _totalNumberOfTestsParser.ParseTotalNumberOfTests(result.Output)
            };
        }

        public TestRunResult CaptureCoverage()
        {
            using (var coverageServer = new CoverageServer())
            {
                var envVars = new Dictionary<string, string>
                {
                    {"Coverage", coverageServer.PipeName }
                };
                var result = LaunchTestProcess(null, envVars);
                if (!coverageServer.WaitReception())
                {
                    _logger.LogWarning("Did not receive mutant coverage data from initial run.");
                }
                else
                {
                    CoveredMutants = coverageServer.RanMutants;
                }
                return result;
            }
        }

        public IEnumerable<int> CoveredMutants { get; private set; }

        public void Dispose()
        {
        }
    }
}