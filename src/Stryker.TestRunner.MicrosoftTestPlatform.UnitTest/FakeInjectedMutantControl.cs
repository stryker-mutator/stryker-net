namespace FakeInjectedHelper;

public static class MutantControl
{
    public const string BlockingCoverageConsumerMarker = Stryker.TestRunner.MicrosoftTestPlatform.Extension.BlockingCoverageFlusher.Marker;

    public static int FlushCount { get; set; }

    public static void FlushCoverageToFileForBlockingConsumer() => FlushCount++;
}
