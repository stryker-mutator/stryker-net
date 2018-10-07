using Stryker.Core.Mutants;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressReporter
    {
        void ReportRunTest(TimeSpan timerElapsed, IReadOnlyMutant mutantTestResult);
        void StartReporting(int totalNumberOfTests);
    }

    public class ProgressReporter : IProgressReporter
    {
        private readonly IConsoleOneLineLogger _testsProgressLogger;

        private readonly IConsoleOneLineLogger _mutantKilledLogger;
        private readonly IConsoleOneLineLogger _mutantSurvivedLogger;
        private readonly IConsoleOneLineLogger _mutantTimeoutLogger;
        private readonly IConsoleOneLineLogger _mutantRuntimeErrorLogger;

        private readonly object _mutex = new object();
        private readonly ConcurrentBag<double> _pastTestRunTimers = new ConcurrentBag<double>();

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

            _testsProgressLogger.StartLog("Tests progress | | {0} / {1} | {2} % | ~ {3}s |", _numberOfTestsRun, _totalNumberOfTests, _numberOfTestsRun * 100 / _totalNumberOfTests, 0);

            _mutantKilledLogger.StartLog("Killed : {0}", 0);
            _mutantSurvivedLogger.StartLog("Survived: {0}", 0);

            _mutantTimeoutLogger.StartLog("Time out : {0}", 0);
            _mutantRuntimeErrorLogger.StartLog("Runtime error : {0}", 0);
        }

        public void ReportRunTest(TimeSpan timerElapsed, IReadOnlyMutant mutantTestResult)
        {
            _pastTestRunTimers.Add(timerElapsed.TotalSeconds);
            lock (_mutex)
            {
                _numberOfTestsRun++;
                ReportMutantTestResult(mutantTestResult);
                var leftTests = _totalNumberOfTests - _numberOfTestsRun;

                var secondsLeft = Math.Round(leftTests * _pastTestRunTimers.Average(), 2);
                var timeLeft = TimeSpan.FromSeconds(secondsLeft);

                _testsProgressLogger.ReplaceLog("Tests progress | | {0} / {1} | {2} % | ~ {3}m{4}s |", _numberOfTestsRun, _totalNumberOfTests, _numberOfTestsRun * 100 / _totalNumberOfTests, timeLeft.Minutes, timeLeft.Seconds);
            }
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