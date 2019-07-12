using System;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.ToolHelpers;
using System.Threading.Tasks.Dataflow;
using System.Collections.Concurrent;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly ConcurrentQueue<VsTestRunner> _availableRunners = new ConcurrentQueue<VsTestRunner>();
        private readonly ICollection<TestCase> _discoveredTests;
        private readonly VsTestHelper _helper = new VsTestHelper();
        private TestCoverageInfos _coverage = new TestCoverageInfos();

        public VsTestRunnerPool(StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo)
        {
            _flags = flags;
            using (var runner = new VsTestRunner(options, _flags, projectInfo, null, null))
            {
                _discoveredTests = runner.DiscoverTests();
            }

            Parallel.For(0, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _availableRunners.Enqueue(new VsTestRunner(options, _flags, projectInfo, _discoveredTests, _coverage, helper:_helper));
            });
        }

        public TestCoverageInfos CoverageMutants => _coverage;

        public IEnumerable<int> CoveredMutants => _coverage.CoveredMutants;

        public TestRunResult RunAll(int? timeoutMs, int? mutationId)
        {
            var runner = TakeRunner();
            TestRunResult result;
            try
            {
                result = runner.RunAll(timeoutMs, mutationId);
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
            var options = new ParallelOptions {MaxDegreeOfParallelism = _availableRunners.Count};
            Parallel.ForEach(_discoveredTests, options, testCase =>
            {
                var runner = TakeRunner();
                try
                {
                    runner.CoverageForTest(testCase);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
                finally
                {
                    ReturnRunner(runner);
                }
            });
            return new TestRunResult {Success = true, TotalNumberOfTests = _discoveredTests.Count};
        }

        private VsTestRunner TakeRunner()
        {
            _availableRunners.TryDequeue(out var testRunner);
            return testRunner;
        }

        private void ReturnRunner(VsTestRunner runner)
        {
            _availableRunners.Enqueue(runner);
        }

        public void Dispose()
        {
            foreach (var testRunner in _availableRunners)
            {
                testRunner.Dispose();
            }
            _helper.Cleanup();
        }
    }
}
