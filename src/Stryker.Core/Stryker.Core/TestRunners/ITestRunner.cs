using System;
using System.Collections.Generic;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners
{
    public delegate bool TestUpdateHandler(IReadOnlyList<Mutant> testedMutants, ITestRunResults results,
        ITestGuids nonCoveringTests);

    public interface ITestRunner : IDisposable
    {
        TestSet DiscoverTests();

        InitialTestRunResult InitialTest();

        IEnumerable<CoverageRunResult> CaptureCoverage();

        TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
    }
}
