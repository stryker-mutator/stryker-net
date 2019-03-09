using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Stryker.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();
        private ICollection<TestCase> _discoveredTests;
        private static readonly ILogger Logger;
         
        static VsTestRunnerPool()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();
        }

        public VsTestRunnerPool(StrykerOptions options, ProjectInfo projectInfo)
        {
            using (var runner = new VsTestRunner(options, projectInfo, null))
            {
                _discoveredTests = runner.DiscoverTests();
            }

            for (var i = 0; i < options.ConcurrentTestrunners; i++)
            {
                _availableRunners.Add(new VsTestRunner(options, projectInfo, _discoveredTests));
            }
        }

        public TestRunResult RunAll(int? timeoutMS, int? mutationId)
        {
            var runner = TakeRunner();

            var result = runner.RunAll(timeoutMS, mutationId);

            ReturnRunner(runner);

            return result;
        }

        public TestRunResult CaptureCoverage()
        {
            var mapping = new Dictionary<TestCase, IEnumerable<int>>(_discoveredTests.Count()); 
            var runner = TakeRunner();
            var result  = runner.CaptureCoverage();
            foreach (var discoveredTest in _discoveredTests)
            {
                mapping[discoveredTest] = runner.CaptureCoverage(discoveredTest);
            }
            ReturnRunner(runner);
            CoveredMutants = runner.CoveredMutants;
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
