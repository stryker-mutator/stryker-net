using Stryker.Core.Testing;
using System.Collections.Generic;
using Stryker.Core.Parsers;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private readonly ITotalNumberOfTestsParser _totalNumberOfTestsParser;
        private string _path { get; set; }
        private IProcessExecutor _processExecutor { get; set; }

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

        public TestRunResult CaptureCoverage(string coverageFilePath)
        {
            var envVars = new Dictionary<string, string>
            {
                {"CoverageFileName", coverageFilePath }
            };
            return LaunchTestProcess(null, envVars);
        }
    }
}