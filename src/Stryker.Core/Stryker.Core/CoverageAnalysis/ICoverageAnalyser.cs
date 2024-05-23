using System.Collections.Generic;
using Stryker.Shared.Initialisation;
using Stryker.Shared.Mutants;
using Stryker.Shared.Tests;

namespace Stryker.Core.CoverageAnalysis;

public interface ICoverageAnalyser
{
    void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<IMutant> mutants, ITestIdentifiers resultFailingTests);
}
