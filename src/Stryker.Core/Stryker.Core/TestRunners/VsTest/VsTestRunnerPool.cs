using Microsoft.Extensions.Logging;
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
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();
        private readonly ILogger _logger;

        public VsTestRunnerPool(StrykerOptions options, ProjectInfo projectInfo)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();

            Parallel.For(0, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _logger.LogTrace("Creating {0} testrunner {1} of {2}", TestRunner.VsTest, i + 1, options.ConcurrentTestrunners);
                _availableRunners.Add(new VsTestRunner(options, projectInfo));
            });
        }

        public int DiscoverNumberOfTests()
        {
            var runner = TakeRunner();

            var result = runner.DiscoverNumberOfTests();

            ReturnRunner(runner);

            return result;
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            var runner = TakeRunner();
            TestRunResult result;

            try
            {
                result = runner.RunAll(timeoutMS, activeMutationId);
            }
            catch (OperationCanceledException)
            {
                ReturnRunner(runner);
                throw;
            }

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
