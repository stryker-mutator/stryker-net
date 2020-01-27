﻿using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.DataCollector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.TestRunners
{
    public class DotnetTestRunner : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly ILogger _logger;
        private readonly string _projectFile;
        private readonly IProcessExecutor _processExecutor;
        private readonly IEnumerable<string> _testBinariesPaths;

        public DotnetTestRunner(string path, IProcessExecutor processProxy, OptimizationFlags flags, IEnumerable<string> testBinariesPaths)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
            _flags = flags;
            _projectFile = path;
            _processExecutor = processProxy;
            CoverageMutants = new TestCoverageInfos();
            _testBinariesPaths = testBinariesPaths;
        }

        public TestCoverageInfos CoverageMutants { get; }

        public IEnumerable<TestDescription> Tests => null;

        public TestRunResult RunAll(int? timeoutMs, IReadOnlyMutant mutant)
        {
            Dictionary<string, string> envVars = mutant == null ? null : 
                new Dictionary<string, string>
            {
                { "ActiveMutation", mutant.Id.ToString() }
            };
            return LaunchTestProcess(timeoutMs, envVars);
        }

        private TestRunResult LaunchTestProcess(int? timeoutMs, IDictionary<string, string> envVars)
        {
            var result = _processExecutor.Start(
                _projectFile,
                "dotnet",
                @"vstest " + string.Join(" ", _testBinariesPaths),
                envVars,
                timeoutMs ?? 0);

            return new TestRunResult
            {
                Success = result.ExitCode == 0,
                ResultMessage = result.Output
            };
        }

        public TestRunResult CaptureCoverage(bool cantUsePipe, bool cantUseUnloadAppDomain)
        {
            if (cantUseUnloadAppDomain)
            {
                _logger.LogWarning("Can't capture the test coverage as the target framework does not support 'AppDomain'. ");
                return new TestRunResult() { Success = true };
            }

            if (cantUsePipe)
            {
                _logger.LogDebug("Target framework does not support NamedPipes. Stryker will use environment variables instead.");
            }
            if (_flags.HasFlag(OptimizationFlags.SkipUncoveredMutants) || _flags.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                var collector = new CoverageCollector();
                collector.SetLogger((message) => _logger.LogTrace(message));
                collector.Init(!cantUsePipe);
                var coverageEnvironment = collector.GetEnvironmentVariables();
                var result = LaunchTestProcess(null, coverageEnvironment);

                var data = collector.RetrieveCoverData();
                var coveredMutants = data.Split(";")[0].Split(",", StringSplitOptions.RemoveEmptyEntries);

                CoverageMutants.DeclareCoveredMutants(coveredMutants.Select(int.Parse));
                return result;
            }

            return new TestRunResult(){Success = true};
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