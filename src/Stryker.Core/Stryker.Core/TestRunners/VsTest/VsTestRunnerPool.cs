using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly Queue<VsTestRunner> _availableRunners = new Queue<VsTestRunner>();
        private readonly IEnumerable<TestCase> _discoveredTests;
        private readonly VsTestHelper _helper = new VsTestHelper();
        private TestCoverageInfos _coverage = new TestCoverageInfos();
        private readonly object _lck = new object();

        public VsTestRunnerPool(StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo)
        {
            _flags = flags;
            using (var runner = new VsTestRunner(options, _flags, projectInfo, null, null))
            {
                _discoveredTests = runner.DiscoverTests();
            }

            Parallel.For(0, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _availableRunners.Enqueue(new VsTestRunner(options, _flags, projectInfo, _discoveredTests, _coverage, helper: _helper));
            });
        }

        public TestCoverageInfos CoverageMutants => _coverage;

        public TestRunResult RunAll(int? timeoutMs, IReadOnlyMutant mutant)
        {
            var runner = TakeRunner();
            TestRunResult result;
            try
            {
                result = runner.RunAll(timeoutMs, mutant);
            }
            finally
            {
                ReturnRunner(runner);
            }
            return result;
        }

        public TestRunResult CaptureCoverage()
        {
            TestRunResult result;
            var needCoverage = _flags.HasFlag(OptimizationFlags.CoverageBasedTest) || _flags.HasFlag(OptimizationFlags.SkipUncoveredMutants);
            if (needCoverage && _flags.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
            {
                return CaptureCoveragePerIsolatedTests();
            }
            var runner = TakeRunner();
            try
            {
                if (needCoverage)
                {
                    result = runner.CaptureCoverage();
                    _coverage = runner.CoverageMutants;
                }
                else
                {
                    result = runner.RunAll(null, null);
                }
            }
            finally
            {
                ReturnRunner(runner);
            }
            return result;
        }

        private TestRunResult CaptureCoveragePerIsolatedTests()
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _availableRunners.Count };
            Parallel.ForEach(_discoveredTests, options, testCase =>
            {
                var runnerLoop = TakeRunner();
                try
                {
                    runnerLoop.CoverageForTest(testCase);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
                finally
                {
                    ReturnRunner(runnerLoop);
                }
            });
            return new TestRunResult { Success = true };
        }

        private VsTestRunner TakeRunner()
        {
            VsTestRunner runner;
            lock (_lck)
            {
                while (!_availableRunners.TryDequeue(out runner))
                {
                    Monitor.Wait(_lck);
                }
            }

            return runner;
        }

        private void ReturnRunner(VsTestRunner runner)
        {
            lock (_lck)
            {
                _availableRunners.Enqueue(runner);
                Monitor.Pulse(_lck);
            }
        }

        public void Dispose()
        {
            foreach (var runner in _availableRunners)
            {
                runner.Dispose();
            }
            _helper.Cleanup();
        }

        public int DiscoverNumberOfTests()
        {
            return _discoveredTests.Count();
        }
    }
}
