using System.Diagnostics;

namespace Stryker.Core.Reporters.Progress
{
    public interface IStopWatchProvider
    {
        void Start();
        long GetElapsedMillisecond();
    }
    public class StopWatchProvider : IStopWatchProvider
    {

        private Stopwatch _watch;

        public void Start()
        {
            _watch = new Stopwatch();
            _watch.Start();
        }

        public long GetElapsedMillisecond()
        {
            return _watch.ElapsedMilliseconds;
        }
    }
}
