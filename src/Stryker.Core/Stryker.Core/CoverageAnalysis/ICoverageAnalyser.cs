using System.Collections.Generic;
using Stryker.Configuration.Initialisation;
using Stryker.Configuration.Mutants;
using Stryker.Configuration.TestRunners;

namespace Stryker.Configuration.CoverageAnalysis
{
    public interface ICoverageAnalyser
    {
        void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<Mutant> mutants, ITestGuids resultFailingTests);
    }
}
