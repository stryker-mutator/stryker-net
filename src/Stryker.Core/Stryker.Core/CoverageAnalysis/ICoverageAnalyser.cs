using System.Collections.Generic;
using Stryker.Abstractions.Initialisation;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.TestRunners;
using Stryker.Core.TestRunners;

namespace Stryker.Core.CoverageAnalysis;

public interface ICoverageAnalyser
{
    void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<IMutant> mutants, ITestGuids resultFailingTests);
}
