using System;
using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.Options;

namespace Stryker.Core.TestRunners
{
    public interface ITestRunner : IDisposable
    {
        TestRunResult RunAll(int? timeoutMS, IReadOnlyMutant activeMutant);
        int DiscoverNumberOfTests();

        TestRunResult CaptureCoverage(bool cantUsePipe, bool cantUseUnloadAppDomain);

        TestCoverageInfos CoverageMutants { get; }

        IEnumerable<TestDescription> Tests { get; }
    }
}