using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stryker.Abstractions.Testing;

public interface ITestRunner : IDisposable
{
    public delegate bool TestUpdateHandler(IReadOnlyList<IMutant> testedMutants,
       ITestIdentifiers failedTests,
       ITestIdentifiers ranTests,
       ITestIdentifiers timedOutTests);

    Task<bool> DiscoverTestsAsync(string assembly);
    Task<bool> DiscoverTestsAsync(string assembly, CancellationToken cancellationToken)
        => DiscoverTestsAsync(assembly);

    ITestSet GetTests(IProjectAndTests project);

    Task<ITestRunResult> InitialTestAsync(IProjectAndTests project);
    Task<ITestRunResult> InitialTestAsync(
        IProjectAndTests project,
        CancellationToken cancellationToken)
        => InitialTestAsync(project);

    IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project);

    Task<ITestRunResult> TestMultipleMutantsAsync(IProjectAndTests project, ITimeoutValueCalculator? timeoutCalc, IReadOnlyList<IMutant> mutants, TestUpdateHandler? update);

    Task<ITestRunResult> TestMultipleMutantsAsync(
        IProjectAndTests project,
        ITimeoutValueCalculator? timeoutCalc,
        IReadOnlyList<IMutant> mutants,
        TestUpdateHandler? update,
        CancellationToken cancellationToken)
        => TestMultipleMutantsAsync(project, timeoutCalc, mutants, update);
}
