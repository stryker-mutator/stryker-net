using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.CoverageAnalysis
{
    public interface ICoverageAnalyser
    {
        void DetermineTestCoverage(IProjectAndTest project, ITestRunner runner, IEnumerable<Mutant> mutants, ITestGuids resultFailingTests);
    }
}
