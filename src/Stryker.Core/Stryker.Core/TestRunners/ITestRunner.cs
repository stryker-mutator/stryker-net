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
        bool DiscoverTests(string assembly);

        TestSet GetTests(IProjectAndTest project);

        TestRunResult InitialTest(IProjectAndTest project);

        IEnumerable<CoverageRunResult> CaptureCoverage(IProjectAndTest project);

        TestRunResult TestMultipleMutants(IProjectAndTest project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
    }
}
