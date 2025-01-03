using System;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.Results;

internal class MsTestResult : ITestResult
{
    public MsTestResult(TimeSpan duration)
    {
        Duration = duration;
    }

    public TimeSpan Duration { get; }
}
