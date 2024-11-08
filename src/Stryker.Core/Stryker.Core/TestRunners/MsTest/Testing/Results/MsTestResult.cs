using System;
using Stryker.Shared.Tests;

namespace Stryker.Core.TestRunners.MsTest.Testing.Results;
internal class MsTestResult : ITestResult
{
    public MsTestResult(TimeSpan duration)
    {
        Duration = duration;
    }

    public TimeSpan Duration { get; }
}
