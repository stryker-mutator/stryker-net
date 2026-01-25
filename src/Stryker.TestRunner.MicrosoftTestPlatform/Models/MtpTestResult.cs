using System.Diagnostics.CodeAnalysis;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed class MtpTestResult : ITestResult
{
    public MtpTestResult(TimeSpan duration)
    {
        Duration = duration;
    }

    public TimeSpan Duration { get; }
}

