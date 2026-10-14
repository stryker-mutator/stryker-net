namespace FakeInjectedHelper;

public static class MutantControl
{
    public const string MtpCoverageConsumerMarker = Stryker.TestRunner.MicrosoftTestPlatform.Extension.CoverageFlusher.Marker;

    public static int FlushCount { get; set; }

    public static void FlushCoverageToFileForDataConsumer() => FlushCount++;
}
