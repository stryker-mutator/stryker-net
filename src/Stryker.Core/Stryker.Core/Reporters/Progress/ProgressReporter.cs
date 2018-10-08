using Stryker.Core.Mutants;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressReporter
    {
        void ReportRunTest(TimeSpan timerElapsed, IReadOnlyMutant mutantTestResult);
        void StartReporting(int totalNumberOfTests);
    }

    public class ProgressReporter : IProgressReporter
    {
        private const int MaxProgressBar = 10;
        private const char ProgressBarDoneToken = '\u2588';
        private const char ProgressBarLeftToken = '-';
        private const int ProgressBarInitialState = -1;

        private readonly IConsoleOneLineLogger _testsProgressLogger;

        private readonly IConsoleOneLineLogger _mutantKilledLogger;
        private readonly IConsoleOneLineLogger _mutantSurvivedLogger;
        private readonly IConsoleOneLineLogger _mutantTimeoutLogger;
        private readonly IConsoleOneLineLogger _mutantRuntimeErrorLogger;

        private readonly object _mutex = new object();
        private readonly ConcurrentBag<TimeSpan> _pastTestRunTimers = new ConcurrentBag<TimeSpan>();

        private int _totalNumberOfTests;
        private int _numberOfTestsRun;

        private int _mutantsKilledCount;
        private int _mutantsSurvivedCount;
        private int _mutantsTimeoutCount;
        private int _mutantsRuntimeErrorCount;

        public ProgressReporter(IConsoleOneLineLogger testsProgressLogger,
                                IConsoleOneLineLogger mutantKilledLogger,
                                IConsoleOneLineLogger mutantSurvivedLogger,
                                IConsoleOneLineLogger mutantTimeoutLogger,
                                IConsoleOneLineLogger mutantRuntimeErrorLogger)
        {
            _testsProgressLogger = testsProgressLogger;
            _mutantKilledLogger = mutantKilledLogger;
            _mutantSurvivedLogger = mutantSurvivedLogger;
            _mutantTimeoutLogger = mutantTimeoutLogger;
            _mutantRuntimeErrorLogger = mutantRuntimeErrorLogger;
        }

        public void StartReporting(int totalNumberOfTests)
        {
            _totalNumberOfTests = totalNumberOfTests;
            var totalNumberOfTestsPercentage = _numberOfTestsRun * 100 / GetTotalNumberOfTestsNotZero();

            var progressBar = GenerateProgressBar(ProgressBarInitialState);

            _testsProgressLogger.StartLog("Tests progress | {0} | {1} / {2} | {3} % | ~ {4}s |",
                                          progressBar,
                                          CorrectNumberOfTestsRunIfTotalNumberOfTestsIsZero(),
                                          _totalNumberOfTests,
                                          totalNumberOfTestsPercentage,
                                          "0");

            _mutantKilledLogger.StartLog("Killed : {0}", 0);
            _mutantSurvivedLogger.StartLog("Survived: {0}", 0);

            _mutantTimeoutLogger.StartLog("Time out : {0}", 0);
            _mutantRuntimeErrorLogger.StartLog("Runtime error : {0}", 0);
        }

        public void ReportRunTest(TimeSpan timerElapsed, IReadOnlyMutant mutantTestResult)
        {
            _pastTestRunTimers.Add(timerElapsed);
            lock (_mutex)
            {
                _numberOfTestsRun++;
                ReportMutantTestResult(mutantTestResult);
                var testsLeft = _totalNumberOfTests - _numberOfTestsRun;

                testsLeft = CorrectTestsLeftIfZero(testsLeft);

                var totalNumberOfTestsPercentage = _numberOfTestsRun * 100 / GetTotalNumberOfTestsNotZero();
                var secondsLeft = Math.Round(testsLeft * _pastTestRunTimers.Average(x => x.TotalSeconds), 2);
                var timeLeft = TimeSpan.FromSeconds(secondsLeft);
                var progressBarCount = totalNumberOfTestsPercentage / 10;

                var stringBuilder = GenerateProgressBar(progressBarCount);

                var formattedTime = string.Format(GetTimeFormat(timeLeft), timeLeft);
                _testsProgressLogger.ReplaceLog("Tests progress | {0} | {1} / {2} | {3} % | ~ {4} |",
                                                stringBuilder, CorrectNumberOfTestsRunIfTotalNumberOfTestsIsZero(), _totalNumberOfTests,
                                                totalNumberOfTestsPercentage, formattedTime);
            }
        }

        private int CorrectNumberOfTestsRunIfTotalNumberOfTestsIsZero()
        {
            return _totalNumberOfTests == 0 ? 0 :_numberOfTestsRun;
        }

        private static int CorrectTestsLeftIfZero(int testsLeft)
        {
            return testsLeft < 0 ? 0 : testsLeft;
        }

        private int GetTotalNumberOfTestsNotZero()
        {
            return _totalNumberOfTests == 0 ? 1 : _totalNumberOfTests;
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

        private void ReportMutantTestResult(IReadOnlyMutant mutantTestResult)
        {
            switch (mutantTestResult.ResultStatus)
            {
                case MutantStatus.Killed:
                    _mutantsKilledCount++;
                    _mutantKilledLogger.ReplaceLog("Killed : {0}", _mutantsKilledCount);
                    break;
                case MutantStatus.Survived:
                    _mutantsSurvivedCount++;
                    _mutantSurvivedLogger.ReplaceLog("Survived: {0}", _mutantsSurvivedCount);
                    break;
                case MutantStatus.RuntimeError:
                    _mutantsRuntimeErrorCount++;
                    _mutantRuntimeErrorLogger.ReplaceLog("Runtime error : {0}", _mutantsRuntimeErrorCount);
                    break;
                case MutantStatus.Timeout:
                    _mutantsTimeoutCount++;
                    _mutantTimeoutLogger.ReplaceLog("Time out : {0}", _mutantsTimeoutCount);
                    break;
            };
        }
    }
}