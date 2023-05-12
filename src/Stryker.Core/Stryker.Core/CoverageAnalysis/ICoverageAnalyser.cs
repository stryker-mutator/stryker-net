using System.Collections.Generic;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;

namespace Stryker.Core.CoverageAnalysis
{
    public interface ICoverageAnalyser
    {
        void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<Mutant> mutants, ITestGuids resultFailingTests);
    }
}
