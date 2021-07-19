using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly IStrykerOptions _options;
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();
        private readonly IDictionary<Guid, VsTestDescription> _vsTests;
        private readonly TestSet _tests;
        private readonly VsTestHelper _helper = new();
        private readonly ILogger _logger;

        public VsTestRunnerPool(IStrykerOptions options, ProjectInfo projectInfo)
        {
            _options = options;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();
            using var runner = new VsTestRunner(options, projectInfo, null, new TestSet(), helper: _helper);
            (_vsTests, _tests) = runner.DiscoverTests(null);
            _availableRunners.Add(runner);

            Parallel.For(1, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _availableRunners.Add(new VsTestRunner(options, projectInfo, _vsTests, _tests, helper: _helper));
            });
        }

        public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutMs, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
        {
            var runner = TakeRunner();

            try
            {
                return mutants == null ? runner.RunAll(timeoutMs, null, update) : runner.TestMultipleMutants(timeoutMs, mutants, update);
            }
            finally
            {
                ReturnRunner(runner);
            }
        }

        public TestRunResult RunAll(ITimeoutValueCalculator timeoutMs, Mutant activeMutant, TestUpdateHandler update)
        {
            return TestMultipleMutants(timeoutMs, activeMutant == null ? null : new List<Mutant> { activeMutant }, update);
        }

        public TestRunResult InitialTest()
        {
            var runner = TakeRunner();

            try
            {
                return runner.InitialTest();
            }
            finally
            {
                ReturnRunner(runner);
            }
        }

        public TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants)
        {
            var needCoverage = _options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest) || _options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants);
            if (needCoverage && _options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
            {
                return CaptureCoveragePerIsolatedTests(mutants);
            }

            var runner = TakeRunner();
            TestRunResult result;

            try
            {
                result = needCoverage ? runner.CaptureCoverage(mutants) : runner.RunAll(null, null, null);
            }
            finally
            {
                ReturnRunner(runner);
            }
            return result;
        }

        private TestRunResult CaptureCoveragePerIsolatedTests(IEnumerable<Mutant> mutants)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _availableRunners.Count };

            Parallel.ForEach(_vsTests.Keys, options, testCase =>
            {
                var runner = TakeRunner();
                try
                {
                    runner.CoverageForOneTest(testCase, mutants);
                }
                catch (Exception e)
                {
                    _logger.LogError("Something went wrong while capturing coverage", e);
                }
                finally
                {
                    ReturnRunner(runner);
                }
            });

            return new TestRunResult (true);
        }

        private VsTestRunner TakeRunner()
        {
            VsTestRunner runner;
            while (!_availableRunners.TryTake(out runner))
            {
                _runnerAvailableHandler.WaitOne();
            }
            return runner;
        }

        private void ReturnRunner(VsTestRunner runner)
        {
            _availableRunners.Add(runner);
            _runnerAvailableHandler.Set();
        }

        public void Dispose()
        {
            foreach (var runner in _availableRunners)
            {
                runner.Dispose();
            }
            _helper.Cleanup();
            _runnerAvailableHandler.Dispose();
        }

        public TestSet DiscoverTests()
        {
            return _tests;
        }
    }
}
