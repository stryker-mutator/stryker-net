using Crayon;
using Stryker.Core.Mutants;
using System;
using System.IO;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressBarReporter
    {
        void ReportInitialState(int totalNumberOfMutants);
        void ReportRunTest(IReadOnlyMutant mutantTestResult);
        void ReportFinalState();
    }

    public class ProgressBarReporter : IProgressBarReporter, IDisposable
    {
        private const string LoggingFormat = "│ Testing mutant {0} / {1} │ K {2} │ S {3} │ T {4} │ {5} │";

        private readonly IProgressBar _progressBar;
        private readonly IStopWatchProvider _stopWatch;
        private readonly TextWriter _consoleWriter;

        private int _totalNumberOfMutants;
        private int _numberOfMutantsRun;
        private bool _disposedValue;

        private int _mutantsKilledCount;
        private int _mutantsSurvivedCount;
        private int _mutantsTimeoutCount; 
        
        public ProgressBarReporter(IProgressBar progressBar, IStopWatchProvider stopWatch, TextWriter consoleWriter = null)
        {
            _progressBar = progressBar;
            _stopWatch = stopWatch;
            _consoleWriter = consoleWriter ?? Console.Out;
        }

        public void ReportInitialState(int totalNumberOfMutants)
        {
            _stopWatch.Start();
            _totalNumberOfMutants = totalNumberOfMutants;

            _progressBar.Start(_totalNumberOfMutants, string.Format(LoggingFormat, 0, _totalNumberOfMutants, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, RemainingTime()));
        }

        public void ReportRunTest(IReadOnlyMutant mutantTestResult)
        {
            _numberOfMutantsRun++;

            switch (mutantTestResult.ResultStatus)
            {
                case MutantStatus.Killed:
                    _mutantsKilledCount++;
                    break;
                case MutantStatus.Survived:
                    _mutantsSurvivedCount++;
                    break;
                case MutantStatus.Timeout:
                    _mutantsTimeoutCount++;
                    break;
            };

            _progressBar.Tick(string.Format(LoggingFormat, _numberOfMutantsRun, _totalNumberOfMutants, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, RemainingTime()));
        }

        public void ReportFinalState()
        {
            Dispose();

            var length = _totalNumberOfMutants.ToString().Length;

            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine($"Killed:   {Output.BrightMagenta(_mutantsKilledCount.ToString().PadLeft(length))}");
            _consoleWriter.WriteLine($"Survived: {Output.BrightMagenta(_mutantsSurvivedCount.ToString().PadLeft(length))}");
            _consoleWriter.WriteLine($"Timeout:  {Output.BrightMagenta(_mutantsTimeoutCount.ToString().PadLeft(length))}");
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _stopWatch?.Stop();
                    _progressBar?.Stop();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
