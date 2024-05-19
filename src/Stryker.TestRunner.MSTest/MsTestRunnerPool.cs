using Stryker.Shared.Coverage;
using Stryker.Shared.Initialisation;
using Stryker.Shared.Mutants;
using Stryker.Shared.Tests;

namespace Stryker.TestRunner.MSTest;
internal class MsTestRunnerPool : ITestRunner
{
    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project) => throw new NotImplementedException();
    public bool DiscoverTests(string assembly) => throw new NotImplementedException();
    public void Dispose() => throw new NotImplementedException();
    public ITestSet GetTests(IProjectAndTests project) => throw new NotImplementedException();
    public ITestRunResult InitialTest(IProjectAndTests project) => throw new NotImplementedException();
    public ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<IMutant> mutants, ITestRunner.TestUpdateHandler update) => throw new NotImplementedException();
}
