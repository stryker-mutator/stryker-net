using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.DataCollector;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly ILogger _logger;
        private readonly string _path;
        private readonly string _projectFile;
        private readonly IProcessExecutor _processExecutor;

        public DotnetTestRunner(string path, IProcessExecutor processProxy, OptimizationFlags flags, ILogger logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();

            _flags = flags;
            _path = Path.GetDirectoryName(FilePathUtils.ConvertPathSeparators(path));
            _projectFile = path;
            _processExecutor = processProxy;
            CoverageMutants = new TestCoverageInfos();
        }

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
            var result = _processExecutor.Start(
                _projectFile,
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
            if (_flags.HasFlag(OptimizationFlags.SkipUncoveredMutants) || _flags.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                var collector = new CoverageCollector();
                collector.SetLogger((message) => _logger.LogTrace(message));
                collector.Init(true);
                var coverageEnvironment = collector.GetEnvironmentVariables();
                var result = LaunchTestProcess(null, coverageEnvironment);

                var data = collector.RetrieveCoverData("full");

                CoverageMutants.DeclareCoveredMutants(data.Split(";")[0].Split(",").Select(int.Parse));
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