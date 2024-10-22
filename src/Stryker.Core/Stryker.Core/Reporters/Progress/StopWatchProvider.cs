using System.Diagnostics;

namespace Stryker.Core.Reporters.Progress;

public interface IStopWatchProvider
{
    void Start();
    void Stop();
    long GetElapsedMillisecond();
}

public class StopWatchProvider : IStopWatchProvider
{
    private Stopwatch _watch;

    public void Start()
    {
        _watch = Stopwatch.StartNew();
    }

    public void Stop()
    {
        _watch?.Stop();
    }

    public long GetElapsedMillisecond()
    {
        return _watch.ElapsedMilliseconds;
    }
}
