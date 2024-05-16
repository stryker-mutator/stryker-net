using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Shared.Tests;

namespace Stryker.TestRunners.VSTest;
internal class VsTestResult : ITestResult
{
    public VsTestResult(TestResult testResult)
    {
        Duration = testResult.Duration;
    }

    public TimeSpan Duration { get; }
}
