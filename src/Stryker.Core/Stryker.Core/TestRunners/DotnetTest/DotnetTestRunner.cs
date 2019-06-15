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
        private readonly OptimizationFlags _flags;

        static DotnetTestRunner()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
        }
        private static ILogger Logger { get; }

        public DotnetTestRunner(string path, IProcessExecutor processProxy, ITotalNumberOfTestsParser totalNumberOfTestsParser, OptimizationFlags flags)
        {
            _totalNumberOfTestsParser = totalNumberOfTestsParser;
            _flags = flags;
            Path = path;
            ProcessExecutor = processProxy;
            CoverageMutants = new TestCoverageInfos();
        }

        private string Path { get; }
        private IProcessExecutor ProcessExecutor { get; }
        public TestCoverageInfos CoverageMutants { get; }

        public TestRunResult RunAll(int? timeoutMs, int? activeMutationId)
        {
            Dictionary<string, string> envVars = activeMutationId == null ? null : 
                new Dictionary<string, string>
            {
                {"ActiveMutation", activeMutationId.ToString() }
            };
            return LaunchTestProcess(timeoutMs, envVars);
        }

        private TestRunResult LaunchTestProcess(int? timeoutMs, IDictionary<string, string> envVars)
        {
            var result = ProcessExecutor.Start(
                Path,
                "dotnet",
                "test --no-build --no-restore",
                envVars,
                timeoutMs ?? 0);

            return new TestRunResult
            {
                Success = result.ExitCode == 0,
                ResultMessage = result.Output,
                TotalNumberOfTests = _totalNumberOfTestsParser.ParseTotalNumberOfTests(result.Output)
            };
        }

        public TestRunResult CaptureCoverage()
        {
            if (_flags.HasFlag(OptimizationFlags.SkipUncoveredMutants))
            {
                var collector = new CoverageCollector();
                collector.SetLogger((message) => Logger.LogTrace(message));
                collector.Init(true);
                var coverageEnvironment = collector.GetEnvironmentVariables();
                var result = LaunchTestProcess(null, coverageEnvironment);

                var data = collector.RetrieveCoverData("full");

                CoverageMutants.DeclareCoveredMutants(data.Split(",").Select(int.Parse));
                return result;
            }
            else
            {
                return LaunchTestProcess(null, null);
            }
        }

        
        public void Dispose()
        {
        }
    }
}