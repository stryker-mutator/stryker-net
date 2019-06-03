using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressBarReporter
    {
        void ReportInitialState(int totalNumberOfTests);
        void ReportRunTest();
    }

    public class ProgressBarReporter : IProgressBarReporter
    {
        private const int MaxProgressBar = 10;
        private const char ProgressBarDoneToken = '\u2588';
        private const char ProgressBarLeftToken = '-';
        private const int ProgressBarInitialState = -1;
        private const string LoggingFormat = "Tests progress | {0} | {1} / {2} | {3} % | {4} |";

        private readonly IConsoleOneLineLogger _testsProgressLogger;
        private readonly IStopWatchProvider _stopWatch; 

        private int _totalNumberOfMutants;
        private int _numberOfMutantsRun;

        public ProgressBarReporter(IConsoleOneLineLogger testsProgressLogger, IStopWatchProvider stopWatch)
        {
            _testsProgressLogger = testsProgressLogger;
            _stopWatch = stopWatch;
        }

        public void ReportInitialState(int totalNumberOfTests)
        {
            _stopWatch.Start();
            _totalNumberOfMutants = totalNumberOfTests;

            _testsProgressLogger.StartLog(LoggingFormat,
                                        GenerateProgressBar(0),
                                        0,
                                        _totalNumberOfMutants,
                                        0,
                                        RemainingTime());

        }

        public void ReportRunTest()
        {
            _numberOfMutantsRun++;

            var totalNumberOfTestsPercentage = _numberOfMutantsRun * 100 / _totalNumberOfMutants;
            var progressBarCount = totalNumberOfTestsPercentage / 10;

            var stringBuilder = GenerateProgressBar(progressBarCount);

            _testsProgressLogger.ReplaceLog(LoggingFormat,
                                            stringBuilder,
                                            _numberOfMutantsRun,
                                            _totalNumberOfMutants,
                                            totalNumberOfTestsPercentage,
                                            RemainingTime());
        }

        private string RemainingTime()
        {
            if (_totalNumberOfMutants == 0 || _numberOfMutantsRun == 0)
            {
                return "NA";
            }

            var elapsed = _stopWatch.GetElapsedMillisecond();
            var remaining = (_totalNumberOfMutants - _numberOfMutantsRun) * elapsed / _numberOfMutantsRun;
            
            return MillisecondsToText(remaining);
        }

        private static string MillisecondsToText(double remaining)
        {
            var span = TimeSpan.FromMilliseconds(remaining);
            if (span.TotalDays >= 1)
            {
                return span.ToString(@"\~d\d\ h\h");
            }

            if (span.TotalHours >= 1)
            {
                return span.ToString(@"\~h\h\ mm\m");
            }

            return span.ToString(@"\~m\m\ ss\s");
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