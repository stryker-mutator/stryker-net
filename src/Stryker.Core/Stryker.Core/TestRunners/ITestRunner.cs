using System;
using System.Collections.Generic;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.TestRunners
{
    public delegate bool TestUpdateHandler(IReadOnlyList<Mutant> testedMutants,
        ITestGuids failedTests,
        ITestGuids ranTests,
        ITestGuids timedOutTests);
    
    public interface ITestRunner : IDisposable
    {
        TestSet DiscoverTests(IProjectAndTest project);

        TestRunResult InitialTest(IProjectAndTest project);

        IEnumerable<CoverageRunResult> CaptureCoverage(IProjectAndTest project);

        TestRunResult TestMultipleMutants(IProjectAndTest project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
    }
}
