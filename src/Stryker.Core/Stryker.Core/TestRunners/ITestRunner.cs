using System;
using System.Collections.Generic;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        int DiscoverNumberOfTests();
        TestRunResult RunAll(int? timeoutMS, int? activeMutationId);

        TestRunResult CaptureCoverage();

        TestCoverageInfos CoverageMutants { get; }

    }
}