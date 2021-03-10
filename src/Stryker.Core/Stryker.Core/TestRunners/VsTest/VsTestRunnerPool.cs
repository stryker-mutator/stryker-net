using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : IMultiTestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();
        private readonly IDictionary<Guid, TestCase> _tests;
        private readonly VsTestHelper _helper = new VsTestHelper();
        private readonly ILogger _logger;

        public VsTestRunnerPool(IStrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();

            _flags = flags;
            using (var runner = new VsTestRunner(options, _flags, projectInfo, null, helper: _helper))
            {
                _tests = runner.DiscoverTests();
                _availableRunners.Add(runner);
            }

            Parallel.For(1, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _availableRunners.Add(new VsTestRunner(options, _flags, projectInfo, _tests, helper: _helper));
            });
        }

        public IEnumerable<TestDescription> Tests => _tests.Values.Select(x => (TestDescription) x);

        public TestRunResult TestMultipleMutants(int? timeoutMs, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
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

        public TestRunResult RunAll(int? timeoutMs, Mutant mutant, TestUpdateHandler update)
        {
            return TestMultipleMutants(timeoutMs, mutant == null ? null : new List<Mutant> {mutant}, update);
        }

        public TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants, bool cantUseAppDomain, bool cantUsePipe)
        {
            var needCoverage = _flags.HasFlag(OptimizationFlags.CoverageBasedTest) || _flags.HasFlag(OptimizationFlags.SkipUncoveredMutants);
            if (needCoverage && _flags.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
            {
                return CaptureCoveragePerIsolatedTests(mutants, cantUseAppDomain, cantUsePipe);
            }

            var runner = TakeRunner();
            TestRunResult result;

            try
            {
                result = needCoverage ? runner.CaptureCoverage(mutants, cantUseAppDomain, cantUsePipe) : runner.RunAll(null, null, null);
            }
            finally
            {
                ReturnRunner(runner);
            }
            return result;
        }

        private TestRunResult CaptureCoveragePerIsolatedTests(IEnumerable<Mutant> mutants, bool cantUseAppDomain, bool cantUsePipe)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _availableRunners.Count };

            Parallel.ForEach(_tests.Values, options, testCase =>
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

        public int DiscoverNumberOfTests()
        {
            return _tests.Count();
        }
    }
}
