using Stryker.Core.Mutants;
using System;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressReporter
    {
        void ReportRunTest(TimeSpan testDuration, IReadOnlyMutant mutantTestResult);
        void ReportInitialState(int totalNumberOfTests);
    }

    public class ProgressReporter : IProgressReporter
    {
        private readonly object _mutex = new object();
        
        private readonly IMutantsResultReporter _mutantsResultReporter;
        private readonly IProgressBarReporter _progressBarReporter;
        public ProgressReporter(IMutantsResultReporter mutantsResultReporter, IProgressBarReporter progressBarReporter)
        {
            _mutantsResultReporter = mutantsResultReporter;
            _progressBarReporter = progressBarReporter;
        }

        public void ReportInitialState(int totalNumberOfTests)
        {
            _progressBarReporter.ReportInitialState(totalNumberOfTests);
            _mutantsResultReporter.ReportInitialState();
        }

        public void ReportRunTest(TimeSpan testDuration, IReadOnlyMutant mutantTestResult)
        {
            lock (_mutex)
            {
                _progressBarReporter.ReportRunTest(testDuration);
                _mutantsResultReporter.ReportMutantTestResult(mutantTestResult);
            }
        }
    }
}