namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Thrown when the test host process exits unexpectedly during a test run (e.g. a mutation caused a
/// fatal fault such as a stack overflow). This is distinct from a timeout: the host is gone, so the
/// run is reported as a runtime error rather than waiting out the full timeout.
/// </summary>
internal sealed class TestHostCrashedException : Exception
{
    public TestHostCrashedException(string message) : base(message)
    {
    }
}
