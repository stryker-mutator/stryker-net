using System;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(int? timeoutMS, IReadOnlyMutant activeMutant);

        TestRunResult CaptureCoverage();

        TestCoverageInfos CoverageMutants { get; }

    }
}