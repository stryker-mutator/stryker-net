using System;
using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(int? timeoutMS, Mutant activeMutant);

        int DiscoverNumberOfTests();

        TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants);

        IEnumerable<TestDescription> Tests { get; }
    }
}