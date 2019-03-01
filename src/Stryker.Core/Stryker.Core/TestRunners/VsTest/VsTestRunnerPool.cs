using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();

        public VsTestRunnerPool(StrykerOptions options, ProjectInfo projectInfo)
        {
            Parallel.For(0, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _availableRunners.Add(new VsTestRunner(options, projectInfo));
            });
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            var runner = TakeRunner();

            TestRunResult result = runner.RunAll(timeoutMS, activeMutationId);

            ReturnRunner(runner);

            return result;
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
            _runnerAvailableHandler.Dispose();
        }
    }
}
