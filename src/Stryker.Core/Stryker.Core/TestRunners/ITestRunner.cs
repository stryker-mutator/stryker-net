using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(int? timeoutMs, Mutant activeMutant);

        int DiscoverNumberOfTests();

        TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants, bool woPipes, bool woAppDomain);

        IEnumerable<TestDescription> Tests { get; }
    }

    public interface IMultiTestRunner : ITestRunner
    {
        TestRunResult TestMultipleMutants(int? timeoutMs, IReadOnlyList<Mutant> mutants);
    }
}