using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();
        private readonly ICollection<TestCase> _discoveredTests;
        private TestCoverageInfos _coverage = new TestCoverageInfos();
         
        public VsTestRunnerPool(StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo)
        {
            _flags = flags;
            using (var runner = new VsTestRunner(options, _flags, projectInfo, null, null))
            {
                _discoveredTests = runner.DiscoverTests();
            }

            for (var i = 0; i < options.ConcurrentTestrunners; i++)
            {
                _availableRunners.Add(new VsTestRunner(options, _flags, projectInfo, _discoveredTests, _coverage));
            }
        }

        public TestRunResult RunAll(int? timeoutMS, int? mutationId)
        {
            var runner = TakeRunner();
            TestRunResult result;
            try
            {
                result = runner.RunAll(timeoutMS, mutationId);
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
            if (_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                
                VsTestRunner runner = null;
                try
                {
                    runner = TakeRunner();
                    result = runner.CaptureCoverage();
                    _coverage = runner.FinalMapping;
                }
                finally
                {
                    ReturnRunner(runner);
                }
                _coverage.Log();
            }
            else
            {
                var runner = TakeRunner();
                try
                {
                    if (_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
                    {
                        result = runner.CaptureCoverage();
                        _coverage.DeclareCoveredMutants(runner.CoveredMutants);
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

            }

            return result;
        }

        public IEnumerable<int> CoveredMutants => _coverage.CoveredMutants;

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
            _runnerAvailableHandler.Dispose();
        }
    }
}
