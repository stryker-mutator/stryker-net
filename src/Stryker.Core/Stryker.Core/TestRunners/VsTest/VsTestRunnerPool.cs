using System;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.ToolHelpers;
using System.Threading.Tasks.Dataflow;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly BufferBlock<VsTestRunner> _availableRunners = new BufferBlock<VsTestRunner>();
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
                _availableRunners.Post(new VsTestRunner(options, _flags, projectInfo, _discoveredTests, _coverage, helper:_helper));
            });
        }

        public TestCoverageInfos CoverageMutants => _coverage;

        public IEnumerable<int> CoveredMutants => _coverage.CoveredMutants;

        public TestRunResult RunAll(int? timeoutMs, int? mutationId)
        {
            var runner = TakeRunner().Result;
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
            var runner = TakeRunner().Result;
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
            Parallel.ForEach(_discoveredTests, options, async testCase =>
            {
                var runnerLoop = await TakeRunner();
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
            return new TestRunResult {Success = true, TotalNumberOfTests = _discoveredTests.Count};
        }

        private async Task<VsTestRunner> TakeRunner()
        {
            return await _availableRunners.ReceiveAsync();
        }

        private void ReturnRunner(VsTestRunner runner)
        {
            _availableRunners.Post(runner);
        }

        public void Dispose()
        {
            IList<VsTestRunner> testRunners = null;
            _availableRunners.TryReceiveAll(out testRunners);
            foreach (var testRunner in testRunners)
            {
                testRunner.Dispose();
            }
            _availableRunners.Complete();
            _helper.Cleanup();
        }
    }
}
