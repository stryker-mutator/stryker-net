using System;
using System.IO;
using Crayon;
using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters.Progress
{
    public interface IProgressBarReporter
    {
        void ReportInitialState(int mutantsToBeTested);
        void ReportRunTest(IReadOnlyMutant mutantTestResult);
        void ReportFinalState();
    }

    public class ProgressBarReporter : IProgressBarReporter, IDisposable
    {
        private const string LoggingFormat = "│ Testing mutant {0} / {1} │ K {2} │ S {3} │ T {4} │ {5} │";

        private readonly IProgressBar _progressBar;
        private readonly IStopWatchProvider _stopWatch;
        private readonly TextWriter _consoleWriter;

        private int _mutantsToBeTested;
        private int _numberOfMutantsRan;
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

        public void ReportInitialState(int mutantsToBeTested)
        {
            _stopWatch.Start();
            _mutantsToBeTested = mutantsToBeTested;

            _progressBar.Start(_mutantsToBeTested, string.Format(LoggingFormat, 0, _mutantsToBeTested, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, RemainingTime()));
        }

        public void ReportRunTest(IReadOnlyMutant mutantTestResult)
        {
            _numberOfMutantsRan++;

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

            _progressBar.Tick(string.Format(LoggingFormat, _numberOfMutantsRan, _mutantsToBeTested, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, RemainingTime()));
        }

        public void ReportFinalState()
        {
            _progressBar.Tick(string.Format(LoggingFormat, _numberOfMutantsRan, _mutantsToBeTested, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, RemainingTime()));
            Dispose();

            var length = _mutantsToBeTested.ToString().Length;

            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine($"Killed:   {Output.Bright.Magenta(_mutantsKilledCount.ToString().PadLeft(length))}");
            _consoleWriter.WriteLine($"Survived: {Output.Bright.Magenta(_mutantsSurvivedCount.ToString().PadLeft(length))}");
            _consoleWriter.WriteLine($"Timeout:  {Output.Bright.Magenta(_mutantsTimeoutCount.ToString().PadLeft(length))}");
        }

        private string RemainingTime()
        {
            if (_mutantsToBeTested == 0 || _numberOfMutantsRan == 0)
            {
                return "NA";
            }

            var elapsed = _stopWatch.GetElapsedMillisecond();
            var remaining = (_mutantsToBeTested - _numberOfMutantsRan) * elapsed / _numberOfMutantsRan;

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
