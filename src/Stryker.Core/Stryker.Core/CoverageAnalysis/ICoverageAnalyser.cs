using System.Collections.Generic;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.CoverageAnalysis;

public interface ICoverageAnalyser
{
    void DetermineTestCoverage(IStrykerOptions options, IProjectAndTests project, ITestRunner runner, IEnumerable<IMutant> mutants, ITestIdentifiers resultFailingTests);
}
