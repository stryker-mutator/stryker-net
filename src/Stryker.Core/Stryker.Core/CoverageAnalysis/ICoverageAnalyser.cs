using System.Collections.Generic;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.CoverageAnalysis;

public interface ICoverageAnalyser
{
    void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<IMutant> mutants, ITestGuids resultFailingTests);
}
