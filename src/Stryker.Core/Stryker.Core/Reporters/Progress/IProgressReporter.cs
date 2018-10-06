using System;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressReporter
    {
        void ReportRunTest(TimeSpan timerElapsed);
        void StartReporting(int totalNumberOfTests);
    }
}