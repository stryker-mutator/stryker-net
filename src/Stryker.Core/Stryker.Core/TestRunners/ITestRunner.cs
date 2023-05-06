using System;
using System.Collections.Generic;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners;

public delegate bool TestUpdateHandler(IReadOnlyList<Mutant> testedMutants,
    ITestGuids failedTests,
    ITestGuids ranTests,
    ITestGuids timedOutTests);

public interface ITestRunner : IDisposable
{
    TestSet DiscoverTests();

    TestRunResult InitialTest();

    IEnumerable<CoverageRunResult> CaptureCoverage();

    TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
}
