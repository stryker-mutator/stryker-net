using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.Shared.Coverage;
using Stryker.Shared.Exceptions;
using Stryker.Shared.Initialisation;
using Stryker.Shared.Mutants;
using Stryker.Shared.Options;
using Stryker.Shared.Tests;
using Stryker.TestRunner.MSTest.Setup;
using Stryker.TestRunner.MSTest.Testing.Results;
using Stryker.TestRunner.MSTest.Testing.Tests;
using System.IO.Abstractions;

namespace Stryker.TestRunner.MSTest;

public class MsTestRunner : ITestRunner
{
    private DiscoveryResult DiscoveryResult { get; }
    private TestProjectLoader TestProjectLoader { get; }

    private IStrykerOptions _strykerOptions;

    public MsTestRunner(IStrykerOptions options, IFileSystem? fileSystem = null)
    {
        _strykerOptions = options;
        DiscoveryResult = new DiscoveryResult();
        TestProjectLoader = new TestProjectLoader(fileSystem);
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        var coverageCollector = CoverageCollector.Create(DiscoveryResult, project.HelperNamespace);

        foreach (var assembly in project.GetTestAssemblies())
        {
            var testProject = TestProjectLoader.Load(assembly);
            var exitCode = testProject.CoverageRun(coverageCollector).GetAwaiter().GetResult();
        }

        return coverageCollector.GetCoverageRunResult(false);
    }

    public bool DiscoverTests(string assembly)
    {
        var testProject = TestProjectLoader.LoadCopy(assembly);
        var exitCode = testProject.Discover(DiscoveryResult, assembly).GetAwaiter().GetResult();
        return DiscoveryResult.TestsPerSource[assembly].Count > 0;
    }

    public ITestSet GetTests(IProjectAndTests project) => DiscoveryResult.GetTestsForSources(project.GetTestAssemblies());

    public ITestRunResult InitialTest(IProjectAndTests project)
    {
        foreach (var test in DiscoveryResult.MsTests.Keys)
        {
            DiscoveryResult.MsTests[test].ClearInitialResult();
        }

        var executedTests = new List<TestNode>();

        foreach (var assembly in project.GetTestAssemblies())
        {
            var testProject = TestProjectLoader.LoadCopy(assembly);
            var exitCode = testProject.InitialTestRun(DiscoveryResult, executedTests).GetAwaiter().GetResult();
        }

        var executed = executedTests
            .Select(tn => tn.Uid.Value)
            .ToHashSet();

        var failed = executedTests
            .Where(tn => tn.Properties.SingleOrDefault<TestNodeStateProperty>() is FailedTestNodeStateProperty)
            .Select(tn => tn.Uid.Value);

        var timedOut = executedTests
            .Where(tn => tn.Properties.SingleOrDefault<TestNodeStateProperty>() is TimeoutTestNodeStateProperty)
            .Select(tn => tn.Uid.Value);

        var duration = TimeSpan.FromTicks(DiscoveryResult.MsTests.Values.Sum(t => t.InitialRunTime.Ticks));

        return TestRunResult.Successful(DiscoveryResult.MsTests.Values,
            new WrappedIdentifierEnumeration(executed),
            new WrappedIdentifierEnumeration(failed),
            new WrappedIdentifierEnumeration(timedOut),
            duration);
    }
    
    public ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<IMutant> mutants, ITestRunner.TestUpdateHandler update)
    {
        // 1. Get mutants and corresponding tests
        var mutantTestsMap = new Dictionary<int, ITestIdentifiers>();
        var testCases = TestCases(mutants, mutantTestsMap);

        if (testCases?.Count == 0)
        {
            return TestRunResult.None(DiscoveryResult.MsTests.Values, "Mutants are not covered by any test!");
        }

        var totalCountOfTests = DiscoveryResult.GetTestsForSources(project.GetTestAssemblies()).Count;

        // 2. Initialize coverage collector
        var coverageCollector = MutantController.Create(project.HelperNamespace, mutantTestsMap);
        var executed = new List<TestNode>();

        // 2. Load Test Projects
        foreach (var assembly in project.GetTestAssemblies())
        {
            var testProject = TestProjectLoader.Load(assembly);
            var exitCode = testProject.MutantRun(coverageCollector, testCases, executed).GetAwaiter().GetResult();
        }

        var tests = executed.Select(tn => tn.Uid.Value).Distinct().Count() >= totalCountOfTests ?
            TestIdentifierList.EveryTest() :
            new WrappedIdentifierEnumeration(executed.Select(tn => tn.Uid.Value));

        var failedTests = executed
            .Where(tn => tn.Properties.SingleOrDefault<TestNodeStateProperty>() is FailedTestNodeStateProperty)
            .Select(tn => tn.Uid.Value);

        var timedOutTests = executed
            .Where(tn => tn.Properties.SingleOrDefault<TestNodeStateProperty>() is TimeoutTestNodeStateProperty)
            .Select(tn => tn.Uid.Value);

        var remainingMutants = update?.Invoke(mutants, new WrappedIdentifierEnumeration(failedTests), tests, new WrappedIdentifierEnumeration(timedOutTests));

        return TestRunResult.None(DiscoveryResult.MsTests.Values, "");
    }

    private ICollection<string>? TestCases(IReadOnlyList<IMutant> mutants, Dictionary<int, ITestIdentifiers> mutantTestsMap)
    {
        if (_strykerOptions.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
        {
            var needAll = false;
            foreach (var mutant in mutants)
            {
                var tests = mutant.AssessingTests;
                needAll = needAll || tests.IsEveryTest;
                mutantTestsMap.Add(mutant.Id, tests);
            }

            return needAll ? null : mutants.SelectMany(m => m.AssessingTests.GetIdentifiers().Select(t => t.ToString())).ToList();
        }

        if (mutants.Count > 1)
        {
            throw new GeneralStrykerException("Internal error: trying to test multiple mutants simultaneously without 'perTest' coverage analysis.");
        }

        mutantTestsMap.Add(mutants[0].Id, TestIdentifierList.EveryTest());
        return null;
    }


    public void Dispose() => throw new NotImplementedException();
}
