using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Stryker.Core.Reporters.Progress
{
    public class ProgressReporter : IProgressReporter
    {
        private readonly IConsoleOneLineLogger _testsProgressLogger;
        private readonly IConsoleOneLineLogger _timeLeftLogger;
        private readonly object _mutex = new object();
        private readonly ConcurrentBag<double> _pastTestRunTimers = new ConcurrentBag<double>();

        private int _totalNumberOfTests;
        private int _numberOfTestsRun;

        public ProgressReporter(IConsoleOneLineLogger testsProgressLogger, IConsoleOneLineLogger timeLeftLogger)
        {
            _testsProgressLogger = testsProgressLogger;
            _timeLeftLogger = timeLeftLogger;
        }

        public void StartReporting(int totalNumberOfTests)
        {
            _totalNumberOfTests = totalNumberOfTests;

            _testsProgressLogger.StartLog($"Tests progress: 0 / {_totalNumberOfTests} [0 % done ]");
            _timeLeftLogger.StartLog($"Time left (approximately): --- s");
        }

        public void ReportRunTest(TimeSpan timerElapsed)
        {
            int leftTests = 0;
            lock (_mutex)
            {
                _numberOfTestsRun++;
                leftTests = _totalNumberOfTests - _numberOfTestsRun;
            }
            _pastTestRunTimers.Add(timerElapsed.TotalSeconds);

            _testsProgressLogger.ReplaceLog($"Tests progress: {_numberOfTestsRun} / {_totalNumberOfTests}" +
                                     $" [ {_numberOfTestsRun * 100 / _totalNumberOfTests} % done ]");

            var secondsLeft = Math.Round(leftTests * _pastTestRunTimers.Average(), 2);
            _timeLeftLogger.ReplaceLog($"Time left (approximately): {secondsLeft} s");
        }
    }
}