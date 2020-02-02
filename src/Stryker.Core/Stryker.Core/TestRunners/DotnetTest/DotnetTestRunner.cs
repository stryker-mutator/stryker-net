using System;
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
        private readonly string _projectFile;
        private readonly IProcessExecutor _processExecutor;
        private readonly ILogger _logger;
        private IEnumerable<string> _testBinariesPaths;

        public DotnetTestRunner(string path, IProcessExecutor processProxy, OptimizationFlags flags, IEnumerable<string> testBinariesPaths)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
            _flags = flags;
            _projectFile = FilePathUtils.NormalizePathSeparators(path);
            _processExecutor = processProxy;
            _testBinariesPaths = testBinariesPaths;
        }

        public IEnumerable<TestDescription> Tests => null;

        public TestRunResult RunAll(int? timeoutMs, Mutant mutant, TestUpdateHandler update)
        {
            var envVars = mutant == null ? null : 
                new Dictionary<string, string>
            {
                { "ActiveMutation", mutant.Id.ToString() }
            };
            try
            {
                var res= LaunchTestProcess(timeoutMs, envVars);
                update?.Invoke(new[] {mutant}, res.RanTests, res.FailingTests);
                return res;
            }
            catch (OperationCanceledException)
            {
                var emptyList = new TestListDescription(null);
                if (mutant != null)
                {
                    mutant.ResultStatus = MutantStatus.Timeout;
                }
                return TestRunResult.TimedOut(emptyList,  emptyList, "time out");
            }
        }

        private TestRunResult LaunchTestProcess(int? timeoutMs, IDictionary<string, string> envVars)
        {
            var result = _processExecutor.Start(
                _projectFile,
                "dotnet",
                @"vstest " + string.Join(" ", _testBinariesPaths),
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

            if (!_flags.HasFlag(OptimizationFlags.SkipUncoveredMutants) &&
                !_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                return new TestRunResult(true);
            }

            var collector = new CoverageCollector();
            collector.SetLogger(message => _logger.LogTrace(message));
            collector.Init(!cantUsePipe);
            var coverageEnvironment = collector.GetEnvironmentVariables();
            var result = LaunchTestProcess(null, coverageEnvironment);

            var data = collector.RetrieveCoverData();
            var testedMutant = data.Split(";")[0].Split(",").Select(int.Parse).ToList();
            foreach (var mutant in mutants)
            {
                mutant.CoveringTests = testedMutant.Contains(mutant.Id) ? TestListDescription.EveryTest() : new TestListDescription(null);
            }

            return result;

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