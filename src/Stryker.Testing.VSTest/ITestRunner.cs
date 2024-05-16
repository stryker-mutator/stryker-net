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

        TestSet GetTests(IProjectAndTests project);

        TestRunResult InitialTest(IProjectAndTests project);

        IEnumerable<CoverageRunResult> CaptureCoverage(IProjectAndTests project);

        TestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
    }
}
