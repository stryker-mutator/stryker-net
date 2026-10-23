namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

/// <summary>
/// A clock that only moves when a test moves it, so code that waits for an interval can be tested
/// without waiting.
/// </summary>
internal sealed class ManualTimeProvider : TimeProvider
{
    private long _timestamp;

    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    public override long GetTimestamp() => Interlocked.Read(ref _timestamp);

    public void Advance(TimeSpan by) => Interlocked.Add(ref _timestamp, by.Ticks);
}
