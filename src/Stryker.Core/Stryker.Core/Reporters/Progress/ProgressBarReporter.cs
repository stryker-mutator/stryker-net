﻿using System;
using System.Collections.Concurrent;
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
        private const string LoggingFormat = "Tests progress | {0} | {1} / {2} | {3} % |";

        private readonly IConsoleOneLineLogger _testsProgressLogger;

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
                                              100);
                return;
            }

            _totalNumberOfTests = totalNumberOfTests;
            var totalNumberOfTestsPercentage = _numberOfTestsRun * 100 / _totalNumberOfTests;

            var progressBar = GenerateProgressBar(ProgressBarInitialState);

            _testsProgressLogger.StartLog(LoggingFormat,
                                          progressBar,
                                          _numberOfTestsRun,
                                          _totalNumberOfTests,
                                          totalNumberOfTestsPercentage);
        }

        public void ReportRunTest()
        {
            if (_totalNumberOfTests == 0)
            {
                _testsProgressLogger.ReplaceLog(LoggingFormat,
                                              GenerateProgressBar(MaxProgressBar),
                                              0,
                                              0,
                                              100);
                return;
            }

            _numberOfTestsRun++;

            var totalNumberOfTestsPercentage = _numberOfTestsRun * 100 / _totalNumberOfTests;
            var progressBarCount = totalNumberOfTestsPercentage / 10;

            var stringBuilder = GenerateProgressBar(progressBarCount);

            _testsProgressLogger.ReplaceLog(LoggingFormat,
                                            stringBuilder, _numberOfTestsRun, _totalNumberOfTests,
                                            totalNumberOfTestsPercentage);
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