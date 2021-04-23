using System;
using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public delegate bool TestUpdateHandler(IReadOnlyList<Mutant> testedMutants,
        ITestGuids failedTests,
        ITestGuids ranTests,
        ITestGuids timedOutTests);

    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(int? timeoutMs, Mutant activeMutant, TestUpdateHandler update);

        int DiscoverNumberOfTests();

        TestRunResult InitialTest();

        TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants, bool cantUseAppDomain, bool cantUsePipe);
    }

    public interface IMultiTestRunner : ITestRunner
    {
        TestRunResult TestMultipleMutants(int? timeoutMs, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
    }
}
