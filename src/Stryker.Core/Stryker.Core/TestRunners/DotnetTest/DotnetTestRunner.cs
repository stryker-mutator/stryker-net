using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
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

        static DotnetTestRunner()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
        }
        private static ILogger Logger { get; }

            _flags = flags;
            Path = path;
            ProcessExecutor = processProxy;
            CoverageMutants = new TestCoverageInfos();
        }

        private string Path { get; }
        private IProcessExecutor ProcessExecutor { get; }
        public TestCoverageInfos CoverageMutants { get; }

        public TestRunResult RunAll(int? timeoutMs, IReadOnlyMutant mutant)
        {
            Dictionary<string, string> envVars = mutant == null ? null : 
                new Dictionary<string, string>
            {
                {"ActiveMutation", mutant.Id.ToString() }
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
                ResultMessage = result.Output
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

        public int DiscoverNumberOfTests()
        {
            return -1;
        }

        public void Dispose()
        {
        }
    }
}