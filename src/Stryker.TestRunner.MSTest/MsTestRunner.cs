using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.Shared.Coverage;
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

    public MsTestRunner(IStrykerOptions options, IFileSystem? fileSystem = null)
    {
        DiscoveryResult = new DiscoveryResult();
        TestProjectLoader = new TestProjectLoader(fileSystem);
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        var assemblies = project.GetTestAssemblies().First();
        var testProject = TestProjectLoader.Load(assemblies);
        return null;
    }

    public bool DiscoverTests(string assembly)
    {
        var testProject = TestProjectLoader.LoadCopy(assembly);

        _ = testProject.Discover(DiscoveryResult, assembly).GetAwaiter().GetResult();

        return DiscoveryResult.TestsPerSource[assembly].Count > 0;
    }

    public ITestSet GetTests(IProjectAndTests project) => DiscoveryResult.GetTestsForSources(project.GetTestAssemblies());

    public ITestRunResult InitialTest(IProjectAndTests project)
    {
        foreach (var test in DiscoveryResult.MsTests.Keys)
        {
            DiscoveryResult.MsTests[test].ClearInitialResult();
        }

        var assemblies = project.GetTestAssemblies();

        var executedTests = new List<TestNode>();

        foreach (var assembly in assemblies)
        {
            var testProject = TestProjectLoader.LoadCopy(assembly);
            _ = testProject.InitialTestRun(DiscoveryResult, executedTests).GetAwaiter().GetResult();
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
            string.Empty,
            [],
            duration);
    }
    
    public ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<IMutant> mutants, ITestRunner.TestUpdateHandler update) => throw new NotImplementedException();
    public void Dispose() => throw new NotImplementedException();
}
