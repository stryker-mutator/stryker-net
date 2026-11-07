namespace Stryker.TestRunner.MicrosoftTestPlatform;

internal sealed class TestHostTerminationException(string message, Exception innerException)
    : Exception(message, innerException);
