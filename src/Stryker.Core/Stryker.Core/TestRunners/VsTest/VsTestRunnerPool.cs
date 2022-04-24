using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;

namespace Stryker.Core.TestRunners.VsTest
{
    public sealed class VsTestRunnerPool : ITestRunner
    {
        private readonly AutoResetEvent _runnerAvailableHandler = new(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new();
        private readonly VsTestHelper _helper = new();
        private readonly ILogger _logger;
        private readonly VsTestContextInformation _context;
        
        public VsTestRunnerPool(StrykerOptions options, ProjectInfo projectInfo, ILogger logger = null, IFileSystem fileSystem = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();
            _context = new VsTestContextInformation(options, projectInfo, _helper, fileSystem);
            _context.Initialize();

            Parallel.For(0, options.Concurrency, (i, _) => 
                 _availableRunners.Add(new VsTestRunner(_context, i, logger)));
        }

        public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
        {
            var runner = TakeRunner();

            try
            {
                return runner.TestMultipleMutants(timeoutCalc, mutants, update);
            }
            finally
            {
                ReturnRunner(runner);
            }
        }

        public TestRunResult InitialTest()
        {
            var runner = TakeRunner();

            try
            {
                return runner.InitialTest();
            }
            finally
            {
                ReturnRunner(runner);
            }
        }

        public TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants)
        {
            TestRunResult result;
            var optimizationMode = _context.Options.OptimizationMode;
            if (!optimizationMode.HasFlag(OptimizationModes.CoverageBasedTest) &&
                !optimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants))
            {
                return new TestRunResult(true);
            }

            if (optimizationMode.HasFlag(OptimizationModes.SmartCoverageCapture))
            {
                var normalTests = _context.VsTests.Keys.ToList();
                var dubiousTests = new List<Guid>();
                // check if we have tests with multiple results that may require isolated capture
                foreach (var vsTestDescription in _context.VsTests.Where( pair => pair.Value.NbSubCases>1))
                {
                    normalTests.Remove(vsTestDescription.Key);
                    dubiousTests.Add(vsTestDescription.Key);
                }
                //    
                var capture = TakeRunner();

                try
                {
                    result = capture.CaptureCoverage(new TestsGuidList(normalTests), mutants);

                }
                finally
                {
                    ReturnRunner(capture);
                }

                return result.Merge(CaptureCoveragePerIsolatedTests(dubiousTests, mutants));
            }
            if (optimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
            {
                return CaptureCoveragePerIsolatedTests(_context.VsTests.Keys, mutants);
            }

            var runner = TakeRunner();

            try
            {
                result = runner.CaptureCoverage(TestsGuidList.EveryTest() , mutants);
            }
            finally
            {
                ReturnRunner(runner);
            }
            return result;
        }

        private TestRunResult CaptureCoveragePerIsolatedTests(IEnumerable<Guid> tests, IEnumerable<Mutant> mutants)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _availableRunners.Count };

            Parallel.ForEach(tests, options, testCase =>
            {
                var runner = TakeRunner();
                try
                {
                    runner.CoverageForOneTest(testCase, mutants);
                }
                catch (Exception e)
                {
                    _logger.LogError("Something went wrong while capturing coverage: {0}", e);
                }
                finally
                {
                    ReturnRunner(runner);
                }
            });

            return new TestRunResult(true);
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
            _helper.Cleanup();
            _runnerAvailableHandler.Dispose();
        }

        public TestSet DiscoverTests() => _context.Tests;
    }
}
