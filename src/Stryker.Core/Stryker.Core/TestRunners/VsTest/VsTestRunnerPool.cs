using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();

        public VsTestRunnerPool(StrykerOptions options, ProjectInfo projectInfo)
        {
            IEnumerable<TestCase> testCases;

            using (var runner = new VsTestRunner(options, projectInfo, null))
            {
                testCases = runner.DiscoverTests();
            }

            for (var i = 0; i < options.ConcurrentTestrunners; i++)
            {
                _availableRunners.Add(new VsTestRunner(options, projectInfo, testCases.Count()));
            }
        }

        public TestRunResult RunAll(int? timeoutMS, int? mutationId)
        {
            var runner = TakeRunner();

            TestRunResult result = runner.RunAll(timeoutMS, mutationId);

            ReturnRunner(runner);

            return result;
        }

        public TestRunResult CaptureCoverage()
        {

            var runner = TakeRunner();
            var result = runner.CaptureCoverage();
            CoveredMutants = runner.CoveredMutants;
            ReturnRunner(runner);
            return result;
        }

        public IEnumerable<int> CoveredMutants { get; private set; }

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
