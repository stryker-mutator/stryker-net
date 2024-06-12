using Stryker.Shared.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Results;
internal class MsTestResult : ITestResult
{
    public MsTestResult(TimeSpan duration)
    {
        Duration = duration;
    }

    public TimeSpan Duration { get; }
}
