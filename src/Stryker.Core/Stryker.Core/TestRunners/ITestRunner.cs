using System;
using System.Collections.Generic;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public delegate bool TestUpdateHandler(IReadOnlyList<Mutant> testedMutants,
        ITestGuids failedTests,
        ITestGuids ranTests,
        ITestGuids timedOutTests);

    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(ITimeoutValueCalculator timeoutMs, Mutant activeMutant, TestUpdateHandler update);

        TestSet DiscoverTests();

        TestRunResult InitialTest();

        TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants);

        TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
    }
}
