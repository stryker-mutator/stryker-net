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
        private IEnumerable<TestCase> _discoveredTests;
        private static readonly ILogger _logger;

        static VsTestRunnerPool()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();
        }

        public VsTestRunnerPool(StrykerOptions options, ProjectInfo projectInfo)
        {
            using (var runner = new VsTestRunner(options, projectInfo, null))
            {
                _discoveredTests = runner.DiscoverTests();
            }

            for (var i = 0; i < options.ConcurrentTestrunners; i++)
            {
                _availableRunners.Add(new VsTestRunner(options, projectInfo, _discoveredTests.Count()));
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
            var mapping = new Dictionary<TestCase, IEnumerable<int>>(_discoveredTests.Count()); 
            var runner = TakeRunner();
            foreach (var discoveredTest in _discoveredTests)
            {
                mapping[discoveredTest] = runner.CaptureCoverage(discoveredTest);
            }
            ReturnRunner(runner);
            CoveredMutants = mapping.Values.SelectMany( x => x);
            LogMapping(mapping);
            return new TestRunResult { Success = true, TotalNumberOfTests = _discoveredTests.Count()};
        }

        private void LogMapping(Dictionary<TestCase, IEnumerable<int>> mapping)
        {
            _logger.LogInformation("Coverage information");
            foreach (var run in mapping)
            {
                var list = new StringBuilder();
                list.AppendJoin(",", run.Value);
                _logger.LogInformation($"Test '{run.Key.DisplayName}' covers [{list}].");
            }
            _logger.LogInformation("*****************");
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
