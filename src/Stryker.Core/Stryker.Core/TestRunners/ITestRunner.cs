using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;

namespace Stryker.Core.TestRunners
{

    public delegate bool TestUpdateHandler(IReadOnlyList<Mutant> testedMutants, TestListDescription ranTests,
        TestListDescription failedTests);

    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(int? timeoutMs, Mutant activeMutant, TestUpdateHandler update);

        int DiscoverNumberOfTests();

        TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants, bool woPipes, bool woAppDomain);

        IEnumerable<TestDescription> Tests { get; }

    }

    public interface IMultiTestRunner : ITestRunner
    {
        TestRunResult TestMultipleMutants(int? timeoutMs, IReadOnlyList<Mutant> mutants, TestUpdateHandler update);
    }
}