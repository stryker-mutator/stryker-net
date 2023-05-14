using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.DataCollector;

namespace Stryker.Core.TestRunners.VsTest
{
    public sealed class VsTestRunnerPool : ITestRunner
    {
        private readonly AutoResetEvent _runnerAvailableHandler = new(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new();
        private readonly ILogger _logger;
        private readonly int _countOfRunners;

        public VsTestContextInformation Context { get; }

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
            Context = vsTestContext;
            _countOfRunners = Math.Max(1, Context.Options.Concurrency);
            Initialize(runnerBuilder);
        }

        [ExcludeFromCodeCoverage(Justification = "It depends on the deployment of VsTest.")]
        public VsTestRunnerPool(StrykerOptions options, IFileSystem fileSystem = null)
        {
            Context = new VsTestContextInformation(options, fileSystem: fileSystem);
            _countOfRunners = Math.Max(1, options.Concurrency);
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();
            Initialize();
        }

        public bool DiscoverTests(string assembly) => Context.AddTestSource(assembly);

        public TestSet GetTests(IProjectAndTests project) => Context.GetTestsForSources(project.GetTestAssemblies());

        public TestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
            => RunThis(runner => runner.TestMultipleMutants(project, timeoutCalc, mutants, update));

        public TestRunResult InitialTest(IProjectAndTests project)
            => RunThis(runner => runner.InitialTest(project));

        public IEnumerable<CoverageRunResult> CaptureCoverage(IProjectAndTests project) => Context.Options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest) ? CaptureCoverageTestByTest(project) : CaptureCoverageInOneGo(project);

        private void Initialize(Func<VsTestContextInformation, int, VsTestRunner> runnerBuilder = null)
        {
            runnerBuilder ??= (context, i) => new VsTestRunner(context, i);
            Task.Run(() =>
                Parallel.For(0, _countOfRunners, (i, _) =>
                {
                    _availableRunners.Add(runnerBuilder(Context, i));
                    _runnerAvailableHandler.Set();
                }));
        }

        private IEnumerable<CoverageRunResult> CaptureCoverageInOneGo(IProjectAndTests project) => ConvertCoverageResult(RunThis(runner => runner.RunCoverageSession(TestGuidsList.EveryTest(), project.GetTestAssemblies(), project.HelperNamespace, project.IsFullFramework).TestResults), false);

        private IEnumerable<CoverageRunResult> CaptureCoverageTestByTest(IProjectAndTests project) => ConvertCoverageResult(CaptureCoveragePerIsolatedTests(project, Context.VsTests.Keys).TestResults, true);

        private IRunResults CaptureCoveragePerIsolatedTests(IProjectAndTests project, IEnumerable<Guid> tests)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _countOfRunners };
            var result = new SimpleRunResults();
            var results = new ConcurrentBag<IRunResults>();
            Parallel.ForEach(tests, options,
                testCase =>
                    results.Add(RunThis(runner => runner.RunCoverageSession(new TestGuidsList(testCase), project.GetTestAssemblies(), project.HelperNamespace, project.IsFullFramework))));

            return results.Aggregate(result, (runResults, singleResult) => runResults.Merge(singleResult));
        }

        private T RunThis<T>(Func<VsTestRunner, T> task)
        {
            VsTestRunner runner;
            while (!_availableRunners.TryTake(out runner))
            {
                _runnerAvailableHandler.WaitOne();
            }

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

        private IEnumerable<CoverageRunResult> ConvertCoverageResult(IEnumerable<TestResult> testResults, bool perIsolatedTest)
        {
            var seenTestCases = new HashSet<Guid>();
            var defaultConfidence = perIsolatedTest ? CoverageConfidence.Exact : CoverageConfidence.Normal;
            var resultCache = new Dictionary<Guid, CoverageRunResult>();
            // initialize the map
            foreach (var testResult in testResults)
            {
                if (testResult.Outcome != TestOutcome.Passed && testResult.Outcome != TestOutcome.Failed)
                {
                    // skip any test result that is not a pass or fail
                    continue;
                }
                if (ConvertSingleResult(testResult, seenTestCases, defaultConfidence,
                        out var coverageRunResult))
                {
                    // we should skip this result
                    continue;
                }

                // ensure we returns only entry per test
                if (resultCache.ContainsKey(coverageRunResult.TestId))
                {
                    resultCache[coverageRunResult.TestId].Merge(coverageRunResult);
                    continue;
                }

                resultCache[coverageRunResult.TestId] = coverageRunResult;

            }

            return resultCache.Values;
        }

        private bool ConvertSingleResult(TestResult testResult, ISet<Guid> seenTestCases,
            CoverageConfidence defaultConfidence, out CoverageRunResult coverageRunResult)
        {
            var (key, value) = testResult.GetProperties().FirstOrDefault(x => x.Key.Id == CoverageCollector.PropertyName);
            var testCaseId = testResult.TestCase.Id;
            var unexpected = false;
            if (!Context.VsTests.ContainsKey(testCaseId))
            {
                _logger.LogWarning(
                    $"VsTestRunner: Coverage analysis run encountered a unexpected test case ({testResult.TestCase.DisplayName}), mutation tests may be inaccurate. Disable coverage analysis if you have doubts.");
                // add the test description to the referential
                Context.VsTests.Add(testCaseId, new VsTestDescription(testResult.TestCase));
                unexpected = true;
            }

            var testDescription = Context.VsTests[testCaseId];
            // is this a suspect test ?
            if (key == null)
            {
                if (seenTestCases.Contains(testCaseId))
                {
                    // this is an extra result. Coverage data is already present in the already parsed result
                    _logger.LogDebug(
                        $"VsTestRunner: Extra result for test {testResult.TestCase.DisplayName}, so no coverage data for it.");
                    coverageRunResult = null;
                    return true;
                }

                // the coverage collector was not able to report anything ==> it has not been tracked by it, so we do not have coverage data
                // ==> we need it to use this test against every mutation
                _logger.LogDebug($"VsTestRunner: No coverage data for {testResult.TestCase.DisplayName}.");
                seenTestCases.Add(testDescription.Id);
                coverageRunResult = new CoverageRunResult(testCaseId, CoverageConfidence.Dubious, Enumerable.Empty<int>(),
                    Enumerable.Empty<int>(), Enumerable.Empty<int>());
            }
            else
            {
                // we have coverage data
                seenTestCases.Add(testDescription.Id);
                var propertyPairValue = value as string;

                coverageRunResult = BuildCoverageRunResultFromCoverageInfo(propertyPairValue, testResult, testCaseId,
                    unexpected ? CoverageConfidence.UnexpectedCase : defaultConfidence);
            }

            return false;
        }

        private CoverageRunResult BuildCoverageRunResultFromCoverageInfo(string propertyPairValue, TestResult testResult,
            Guid testCaseId, CoverageConfidence level)
        {
            IEnumerable<int> coveredMutants;
            IEnumerable<int> staticMutants;
            IEnumerable<int> leakedMutants;

            if (string.IsNullOrWhiteSpace(propertyPairValue))
            {
                // do not attempt to parse empty strings
                _logger.LogDebug($"VsTestRunner: Test {testResult.TestCase.DisplayName} does not cover any mutation.");
                coveredMutants = Enumerable.Empty<int>();
                staticMutants = Enumerable.Empty<int>();
            }
            else
            {
                var parts = propertyPairValue.Split(';');
                coveredMutants = string.IsNullOrEmpty(parts[0])
                    ? Enumerable.Empty<int>()
                    : parts[0].Split(',').Select(int.Parse);
                // we identify mutants that are part of static code, unless we performed pertest capture
                staticMutants = parts.Length == 1 || string.IsNullOrEmpty(parts[1]) ||
                                Context.Options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest)
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
