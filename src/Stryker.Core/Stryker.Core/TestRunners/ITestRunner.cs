using System;
using System.Collections.Generic;
using Stryker.Abstractions;
using Stryker.Abstractions.Initialisation;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.TestRunners;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners;

public delegate bool TestUpdateHandler(IReadOnlyList<IMutant> testedMutants,
    ITestGuids failedTests,
    ITestGuids ranTests,
    ITestGuids timedOutTests);

public interface ITestRunner : IDisposable
{
    bool DiscoverTests(string assembly);

    TestSet GetTests(IProjectAndTests project);

    TestRunResult InitialTest(IProjectAndTests project);

    IEnumerable<CoverageRunResult> CaptureCoverage(IProjectAndTests project);

    TestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<IMutant> mutants, TestUpdateHandler update);
}
