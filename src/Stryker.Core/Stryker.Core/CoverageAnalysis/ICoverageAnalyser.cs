using System.Collections.Generic;
using Stryker.Abstractions.Initialisation;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.TestRunners;

namespace Stryker.Abstractions.CoverageAnalysis
{
    public interface ICoverageAnalyser
    {
        void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<Mutant> mutants, ITestGuids resultFailingTests);
    }
}
