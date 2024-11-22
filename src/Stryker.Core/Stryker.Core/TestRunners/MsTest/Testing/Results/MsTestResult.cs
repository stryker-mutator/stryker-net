using System;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.TestRunners.MsTest.Testing.Results;

internal class MsTestResult : ITestResult
{
    public MsTestResult(TimeSpan duration)
    {
        Duration = duration;
    }

    public TimeSpan Duration { get; }
}
