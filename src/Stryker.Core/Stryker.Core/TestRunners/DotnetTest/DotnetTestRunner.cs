using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.DataCollector;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly ILogger _logger;
        private readonly string _path;
        private readonly IProcessExecutor _processExecutor;
        public TestCoverageInfos CoverageMutants { get; }

        public DotnetTestRunner(string path, IProcessExecutor processProxy, OptimizationFlags flags, ILogger logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();

            _flags = flags;
            _path = path;
            _processExecutor = processProxy;
            CoverageMutants = new TestCoverageInfos();
        }

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
            var result = _processExecutor.Start(
                _path,
                "dotnet",
                "test --no-build --no-restore",
                envVars,
                timeoutMs ?? 0);

            return new TestRunResult
            {
                Success = result.ExitCode == 0,
                ResultMessage = result.Output
            };
        }

        public TestRunResult CaptureCoverage()
        {
            if (_flags.HasFlag(OptimizationFlags.SkipUncoveredMutants))
            {
                var collector = new CoverageCollector();
                collector.SetLogger((message) => _logger.LogTrace(message));
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

        public int DiscoverNumberOfTests()
        {
            return -1;
        }

        public void Dispose()
        {
        }
    }
}