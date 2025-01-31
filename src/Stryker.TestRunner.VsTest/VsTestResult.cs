using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.VsTest;

internal class VsTestResult : ITestResult
{
    public VsTestResult(TestResult testResult)
    {
        Duration = testResult.Duration;
    }

    public TimeSpan Duration { get; }
}
