﻿using Microsoft.Extensions.Logging;
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
        private readonly string _projectFile;
        private readonly IProcessExecutor _processExecutor;
        private readonly ILogger _logger;

        public DotnetTestRunner(string path, IProcessExecutor processProxy, OptimizationFlags flags, ILogger logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
            _flags = flags;
            _projectFile = FilePathUtils.NormalizePathSeparators(path);
            _processExecutor = processProxy;
        }

        public IEnumerable<TestDescription> Tests => null;

        public TestRunResult RunAll(int? timeoutMs, Mutant mutant)
        {
            var envVars = mutant == null ? null : 
                new Dictionary<string, string>
            {
                {"ActiveMutation", mutant.Id.ToString() }
            };
            var result =  LaunchTestProcess(timeoutMs, envVars);
            mutant?.AnalyzeTestRun(result.FailingTests.GetList(), result.RanTests);
            return result;
        }

        private TestRunResult LaunchTestProcess(int? timeoutMs, IDictionary<string, string> envVars)
        {
            var result = _processExecutor.Start(
                _projectFile,
                "dotnet",
                "test --no-build --no-restore",
                envVars,
                timeoutMs ?? 0);

            return new TestRunResult(result.ExitCode == 0, result.Output);
        }

        public TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants, bool cantUseUnloadAppDomain, bool cantUsePipe)
        {
            if (cantUseUnloadAppDomain)
            {
                _logger.LogWarning("Can't capture the test coverage as the target framework does not support 'AppDomain'. ");
                return new TestRunResult(true);
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

                var data = collector.RetrieveCoverData("full");
                var testedMutant = data.Split(";")[0].Split(",").Select(int.Parse).ToList();
                foreach (var mutant in mutants)
                {
                    mutant.CoveringTests = testedMutant.Contains(mutant.Id) ? TestListDescription.EveryTest() : new TestListDescription(null);
                }

                return result;
            }

            return new TestRunResult(true);
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