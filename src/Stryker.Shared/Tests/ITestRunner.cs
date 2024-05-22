using Stryker.Shared.Coverage;
using Stryker.Shared.Initialisation;
using Stryker.Shared.Mutants;

namespace Stryker.Shared.Tests;

public interface ITestRunner : IDisposable
{
    public delegate bool TestUpdateHandler(IReadOnlyList<IMutant> testedMutants,
       ITestIdentifiers failedTests,
       ITestIdentifiers ranTests,
       ITestIdentifiers timedOutTests);

    bool DiscoverTests(string assembly);

    ITestSet GetTests(IProjectAndTests project);

    ITestRunResult InitialTest(IProjectAndTests project);

    IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project);

    ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<IMutant> mutants, TestUpdateHandler update);
}
