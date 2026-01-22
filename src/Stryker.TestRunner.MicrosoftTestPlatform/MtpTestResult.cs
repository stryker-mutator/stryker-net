using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

public sealed class MtpTestResult : ITestResult
{
    public MtpTestResult(TimeSpan duration)
    {
        Duration = duration;
    }

    public TimeSpan Duration { get; }
}

