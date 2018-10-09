using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressBarReporter
    {
        void ReportInitialState(int totalNumberOfTests);
        void ReportRunTest(TimeSpan testDuration);
    }

    public class ProgressBarReporter : IProgressBarReporter
    {
        private const int MaxProgressBar = 10;
        private const char ProgressBarDoneToken = '\u2588';
        private const char ProgressBarLeftToken = '-';
        private const int ProgressBarInitialState = -1;
        private const string LoggingFormat = "Tests progress | {0} | {1} / {2} | {3} % | ~ {4} |";

        private readonly IConsoleOneLineLogger _testsProgressLogger;

        private readonly ConcurrentBag<TimeSpan> _pastTestRunTimers = new ConcurrentBag<TimeSpan>();

        private int _totalNumberOfTests;
        private int _numberOfTestsRun;

        public ProgressBarReporter(IConsoleOneLineLogger testsProgressLogger)
        {
            _testsProgressLogger = testsProgressLogger;
        }

        public void ReportInitialState(int totalNumberOfTests)
        {
            if (totalNumberOfTests == 0)
            {
                _testsProgressLogger.StartLog(LoggingFormat,
                                              GenerateProgressBar(MaxProgressBar),
                                              0,
                                              0,
                                              100,
                                              "0 s");
                return;
            }

            _totalNumberOfTests = totalNumberOfTests;
            var totalNumberOfTestsPercentage = _numberOfTestsRun * 100 / _totalNumberOfTests;

            var progressBar = GenerateProgressBar(ProgressBarInitialState);

            _testsProgressLogger.StartLog(LoggingFormat,
                                          progressBar,
                                          _numberOfTestsRun,
                                          _totalNumberOfTests,
                                          totalNumberOfTestsPercentage,
                                          "0 s");
        }

        public void ReportRunTest(TimeSpan testDuration)
        {
            if (_totalNumberOfTests == 0)
            {
                _testsProgressLogger.ReplaceLog(LoggingFormat,
                                              GenerateProgressBar(MaxProgressBar),
                                              0,
                                              0,
                                              100,
                                              "0 s");
                return;
            }

            _pastTestRunTimers.Add(testDuration);
            _numberOfTestsRun++;
            var testsLeft = _totalNumberOfTests - _numberOfTestsRun;

            var totalNumberOfTestsPercentage = _numberOfTestsRun * 100 / _totalNumberOfTests;
            var secondsLeft = Math.Round(testsLeft * _pastTestRunTimers.Average(x => x.TotalSeconds), 2);
            var timeLeft = TimeSpan.FromSeconds(secondsLeft);
            var progressBarCount = totalNumberOfTestsPercentage / 10;

            var stringBuilder = GenerateProgressBar(progressBarCount);

            var formattedTime = string.Format(GetTimeFormat(timeLeft), timeLeft);
            _testsProgressLogger.ReplaceLog(LoggingFormat,
                                            stringBuilder, _numberOfTestsRun, _totalNumberOfTests,
                                            totalNumberOfTestsPercentage, formattedTime);
        }

        private static string GetTimeFormat(TimeSpan timeLeft)
        {
            if (timeLeft.Days > 0)
            {
                return "{0:%d} days {0:%h} h {0:%m} m {0:%s} s";
            }

            if (timeLeft.Hours > 0)
            {
                return "{0:%h} h {0:%m} m {0:%s} s";
            }

            if (timeLeft.Minutes > 0)
            {
                return "{0:%m} m {0:%s} s";
            }

            return "{0:%s} s";
        }

        private string GenerateProgressBar(int progressBarCount)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < MaxProgressBar; i++)
            {
                if (i < progressBarCount)
                {
                    stringBuilder.Append(ProgressBarDoneToken);
                    continue;
                }

                stringBuilder.Append(ProgressBarLeftToken);
            }

            return stringBuilder.ToString();
        }
    }
}