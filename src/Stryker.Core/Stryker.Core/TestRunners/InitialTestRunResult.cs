using System;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners;

public class InitialTestRunResult
{
    public ITestGuids RanTests {get;}

    public ITestGuids FailedTests { get; }

    public TimeSpan Duration { get; }

    public InitialTestRunResult(ITestGuids ranTests, ITestGuids failedTests, TimeSpan duration)
    {
        RanTests = ranTests;
        FailedTests = failedTests;
        Duration = duration;
    }
}
