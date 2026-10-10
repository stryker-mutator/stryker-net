using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stryker.Abstractions.Testing;

public interface ITestRunner : IDisposable
{
    public delegate bool TestUpdateHandler(IReadOnlyList<IMutant> testedMutants,
       ITestIdentifiers failedTests,
       ITestIdentifiers ranTests,
       ITestIdentifiers timedOutTests);

    /// <summary>
    /// Discovers tests using the configured test-case filter.
    /// </summary>
    /// <param name="assembly">The path to the test assembly to discover tests in.</param>
    /// <returns>
    /// A task whose result is <see langword="true"/> when discovery completes successfully with tests,
    /// or <see langword="false"/> when discovery completes successfully without tests.
    /// </returns>
    /// <exception cref="Exceptions.InputException">
    /// Test discovery fails, for example because the test assembly cannot be loaded or the test host fails.
    /// </exception>
    /// <exception cref="OperationCanceledException">Test discovery is canceled.</exception>
    Task<bool> DiscoverTestsAsync(string assembly);

    ITestSet GetTests(IProjectAndTests project);

    Task<ITestRunResult> InitialTestAsync(IProjectAndTests project);

    IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project);

    Task<ITestRunResult> TestMultipleMutantsAsync(IProjectAndTests project, ITimeoutValueCalculator? timeoutCalc, IReadOnlyList<IMutant> mutants, TestUpdateHandler? update);
}
