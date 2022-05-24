using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.DataCollector;

namespace Stryker.Core.TestRunners.VsTest
{   
    public sealed class VsTestRunnerPool : ITestRunner
    {
        private readonly AutoResetEvent _runnerAvailableHandler = new(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new();
        private readonly ILogger _logger;
        private readonly VsTestContextInformation _context;
        
        /// <summary>
        /// this constructor is for test purposes
        /// </summary>
        /// <param name="vsTestContext"></param>
        /// <param name="forcedLogger"></param>
        /// <param name="runnerBuilder"></param>
        public VsTestRunnerPool(VsTestContextInformation vsTestContext,
            ILogger forcedLogger,
            Func<VsTestContextInformation, int, VsTestRunner> runnerBuilder)
        {
            _logger = forcedLogger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();
            _context = vsTestContext;
            runnerBuilder ??= (context, i) => new VsTestRunner(context, i);
            Parallel.For(0, Math.Max(1, _context.Options.Concurrency), (i, _) => 
                 _availableRunners.Add(runnerBuilder(_context, i)));
        }

        public VsTestRunnerPool(StrykerOptions options,
            ProjectInfo projectInfo)
        {
            _context = new VsTestContextInformation(options, projectInfo);
            _context.Initialize();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();
            Parallel.For(0, Math.Max(1, _context.Options.Concurrency), (i, _) => 
                 _availableRunners.Add(new VsTestRunner(_context, i)));
        }

        public TestSet DiscoverTests() => _context.Tests;

        public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
            => RunThis(runner => runner.TestMultipleMutants(timeoutCalc, mutants, update));

        public TestRunResult InitialTest()
            => RunThis(runner => runner.InitialTest());

        public IEnumerable<CoverageRunResult> CaptureCoverage()
        {
            var optimizationMode = _context.Options.OptimizationMode;

            IRunResults resultsToParse;
            if (optimizationMode.HasFlag(OptimizationModes.SmartCoverageCapture))
            {
                resultsToParse = SmartCoverageCapture();
            }
            else if (optimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
            {
                resultsToParse = CaptureCoveragePerIsolatedTests(_context.VsTests.Keys);
            }
            else
            {
                resultsToParse = RunThis(runner => runner.RunCoverageSession(TestsGuidList.EveryTest()));
            }

            return ConvertCoverageResult(resultsToParse.TestResults, false);
        }

        private SimpleRunResults SmartCoverageCapture()
        {
            var normalTests = _context.VsTests.Keys.ToList();
            var dubiousTests = new List<Guid>();

            // check if we have tests with multiple results that may require isolated capture
            foreach (var suspiciousCase in _context.VsTests.Values.Where(pair => pair.NbSubCases > 1))
            {
                var similar = _context.FindTestCasesWithinDataSource(suspiciousCase);
                foreach (var description in similar)
                {
                    normalTests.Remove(description.Id);
                    dubiousTests.Add(description.Id);
                    _logger.LogDebug($"Coverage for test {description.Case.DisplayName} will be established in isolation.");
                }
            }

            var aggregator = new SimpleRunResults();
            aggregator.Merge(
                RunThis(
                    runner => runner.RunCoverageSession(new TestsGuidList(normalTests))));
            List<TestResult> suspiciousResults = new();
            // now scan if we find tests with 'early' coverage
            foreach (var result in aggregator.TestResults.Where(result => result.Properties.Any(x => x.Id == CoverageCollector.OutOfTestsPropertyName)))
            {
                suspiciousResults.Add(result);
                var similar = _context.FindTestCasesWithinDataSource(_context.VsTests[result.TestCase.Id]);
                // we have a leak
                foreach (var description in similar)
                {
                    normalTests.Remove(description.Id);
                    dubiousTests.Add(description.Id);
                    _logger.LogDebug($"Coverage for test {description.Case.DisplayName} will be established in isolation.");
                }

                dubiousTests.Add(result.TestCase.Id);
            }

            aggregator.Merge(CaptureCoveragePerIsolatedTests(dubiousTests));
            return aggregator;
        }

        private IRunResults CaptureCoveragePerIsolatedTests(IEnumerable<Guid> tests)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _availableRunners.Count };
            var result = new SimpleRunResults();
            var results = new ConcurrentBag<IRunResults>();
            Parallel.ForEach(tests, options,
                testCase =>
                    results.Add(RunThis(runner => runner.RunCoverageSession(new TestsGuidList(testCase)))));

            return results.Aggregate(result, (runResults, singleResult) => runResults.Merge(singleResult));
        }

        private T RunThis<T>(Func<VsTestRunner, T> task)
        {
            VsTestRunner runner1;
            while (!_availableRunners.TryTake(out runner1))
            {
                _runnerAvailableHandler.WaitOne();
            }

            var runner = runner1;

            try
            {
                return task(runner);
            }
            finally
            {
                _availableRunners.Add(runner);
                _runnerAvailableHandler.Set();
            }
        }

        public void Dispose()
        {
            foreach (var runner in _availableRunners)
            {
                runner.Dispose();
            }
            _runnerAvailableHandler.Dispose();
        }

        private IEnumerable<CoverageRunResult> ConvertCoverageResult(ICollection<TestResult> testResults, bool perIsolatedTest)
        {
            var seenTestCases = new HashSet<Guid>();
            var dynamicTestCases = new HashSet<Guid>();
            var results = new List<CoverageRunResult>(testResults.Count);
            // initialize the map
            foreach (var testResult in testResults)
            {
                var (key, value) = testResult.GetProperties().FirstOrDefault(x => x.Key.Id == CoverageCollector.PropertyName);
                var testCaseId = testResult.TestCase.Id;
                if (!_context.VsTests.ContainsKey(testCaseId))
                {
                    _logger.LogWarning($"VsTestRunner: Coverage analysis run encountered a unexpected test case ({testResult.TestCase.DisplayName}), mutation tests may be inaccurate. Disable coverage analysis if you have doubts.");
                    // add the test description
                    _context.VsTests.Add(testCaseId, new VsTestDescription(testResult.TestCase));
                }
                var testDescription = _context.VsTests[testCaseId];
                if (key == null)
                {
                    // the coverage collector was not able to report anything ==> it has not been tracked by it, so we do not have coverage data
                    // ==> we need it to use this test against every mutation
                    if (seenTestCases.Contains(testCaseId) ||
                        dynamicTestCases.Contains(testCaseId))
                    {
                        _logger.LogDebug($"VsTestRunner: Test {testResult.TestCase.DisplayName} was not tracked, so no coverage data for it.");
                        continue;
                    }

                    // this is a suspect test
                    dynamicTestCases.Add(testDescription.Id);
                    results.Add(new CoverageRunResult(testCaseId, CoverageConfidence.Dubious, Enumerable.Empty<int>(), Enumerable.Empty<int>(), Enumerable.Empty<int>()));
                }
                else if (value != null)
                {
                    // we have coverage data
                    seenTestCases.Add(testDescription.Id);
                    var propertyPairValue = value as string;

                    results.Add(BuildCoverageRunResultFromCoverageInfo(propertyPairValue, testResult, testCaseId, perIsolatedTest ? CoverageConfidence.Exact : CoverageConfidence.Normal));
                }
            }

            return results;
        }

        private CoverageRunResult BuildCoverageRunResultFromCoverageInfo(string propertyPairValue, TestResult testResult,
            Guid testCaseId, CoverageConfidence level)
        {
            IEnumerable<int> coveredMutants;
            IEnumerable<int> staticMutants;
            IEnumerable<int> leakedMutants;

            if (string.IsNullOrWhiteSpace(propertyPairValue))
            {
                _logger.LogDebug($"VsTestRunner: Test {testResult.TestCase.DisplayName} does not cover any mutation.");
                coveredMutants = null;
                staticMutants = null;
            }
            else
            {
                var parts = propertyPairValue.Split(';');
                coveredMutants = string.IsNullOrEmpty(parts[0])
                    ? Enumerable.Empty<int>()
                    : parts[0].Split(',').Select(int.Parse);
                // we identify mutants that are part of static code, unless we performed pertest capture
                staticMutants = (parts.Length == 1 || string.IsNullOrEmpty(parts[1]) ||
                                 _context.Options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
                    ? Enumerable.Empty<int>()
                    : parts[1].Split(',').Select(int.Parse);
            }

            // look for suspicious mutants
            var (testProperty, mutantOutsideTests) = testResult.GetProperties()
                .FirstOrDefault(x => x.Key.Id == CoverageCollector.OutOfTestsPropertyName);
            if (testProperty != null)
            {
                // we have some mutations that appeared outside any test, probably some run time test case generation, or some async logic.
                propertyPairValue = mutantOutsideTests as string;
                leakedMutants = string.IsNullOrEmpty(propertyPairValue)
                    ? Enumerable.Empty<int>()
                    : propertyPairValue.Split(',').Select(int.Parse);
                _logger.LogWarning(
                    $"VsTestRunner: Some mutations were executed outside any test (mutation ids: {propertyPairValue}).");
            }
            else
            {
                leakedMutants = Enumerable.Empty<int>();
            }

            return new CoverageRunResult(testCaseId, level, coveredMutants, staticMutants, leakedMutants);
        }
    }
}
