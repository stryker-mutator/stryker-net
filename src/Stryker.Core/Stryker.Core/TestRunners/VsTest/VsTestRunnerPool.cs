﻿using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

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
                _discoveredTests = runner.DiscoverTests();

            Parallel.For(0, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _availableRunners.Add(new VsTestRunner(options, _flags, projectInfo, _discoveredTests, _coverage));
                _availableRunners.Add(new VsTestRunner(options, projectInfo));
            });
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
            }
            catch (OperationCanceledException)
            {
                ReturnRunner(runner);
                throw;
            }

            var runner = TakeRunner();
            try
            {
                if (_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
                {
                    result = runner.CaptureCoverage();
                    _coverage = runner.FinalMapping;
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
