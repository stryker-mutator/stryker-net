using Stryker.Core.Parsers;
using Stryker.Core.Testing;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.DataCollector;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private readonly ITotalNumberOfTestsParser _totalNumberOfTestsParser;
        private string _path { get; }
        private IProcessExecutor _processExecutor { get; }
        private static ILogger _logger { get; }
        private readonly OptimizationFlags _flags;

        static DotnetTestRunner()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
        }

        public DotnetTestRunner(string path, IProcessExecutor processProxy, ITotalNumberOfTestsParser totalNumberOfTestsParser, OptimizationFlags flags)
        {
            _totalNumberOfTestsParser = totalNumberOfTestsParser;
            _path = path;
            _processExecutor = processProxy;
            _flags = flags & ~OptimizationFlags.UseEnvVariable;
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

        private TestRunResult LaunchTestProcess(int? timeoutMS, IDictionary<string, string> envVars)
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
            var collector = new CoverageCollector();
            collector.Init(true);
            collector.SetLogger((message) => _logger.LogDebug(message));
            var coverageEnvironment = collector.GetEnvironmentVariables();
            var result = LaunchTestProcess(null, coverageEnvironment);

            var data = collector.RetrieveCoverData("full");

            CoveredMutants = data.Split(",").Select(int.Parse); 
            return result;
        }

        public TestCoverageInfos CoverageMutants => null;

        public IEnumerable<int> CoveredMutants { get; private set; }

        public void Dispose()
        {
        }
    }
}