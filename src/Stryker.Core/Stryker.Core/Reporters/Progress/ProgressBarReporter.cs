using System;
using System.Timers;
using Spectre.Console;
using Stryker.Abstractions;

namespace Stryker.Core.Reporters.Progress;

public interface IProgressBarReporter
{
    int CoverageTestsProcessed { get; }

    void ReportInitialState(int mutantsToBeTested);
    void ReportRunTest(IReadOnlyMutant mutantTestResult);
    void ReportFinalState();

    void ReportCoverageAnalysisStarted(int totalTests);
    void ReportCoverageAnalysisProgress(int testsProcessed);
    void ReportCoverageAnalysisCompleted();
}

public class ProgressBarReporter : IProgressBarReporter, IDisposable
{
    private const string LoggingFormat = "│ Testing mutant {0} / {1} │ K {2} │ S {3} │ T {4} │ E {5} │ {6} │";

    private readonly IProgressBar _progressBar;
    private readonly IStopWatchProvider _stopWatch;
    private readonly IAnsiConsole _console;

    private int _mutantsToBeTested;
    private int _numberOfMutantsRan;
    private bool _disposedValue;

    private int _mutantsKilledCount;
    private int _mutantsSurvivedCount;
    private int _mutantsTimeoutCount;
    private int _mutantsRuntimeErrorCount;

    private readonly string[] _spinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    private int _spinnerIndex;
    private bool _coverageAnalysisActive;
    private int _coverageTotalTests;
    public int CoverageTestsProcessed { get; private set; }
    private Timer _spinnerTimer;

    public ProgressBarReporter(IProgressBar progressBar, IStopWatchProvider stopWatch, IAnsiConsole console = null)
    {
        _progressBar = progressBar;
        _stopWatch = stopWatch;
        _console = console ?? AnsiConsole.Console;
    }

    public void ReportInitialState(int mutantsToBeTested)
    {
        _stopWatch.Start();
        _mutantsToBeTested = mutantsToBeTested;

        _progressBar.Start(_mutantsToBeTested, string.Format(LoggingFormat, 0, _mutantsToBeTested, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, _mutantsRuntimeErrorCount, RemainingTime()));
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
            case MutantStatus.RuntimeError:
                _mutantsRuntimeErrorCount++;
                break;
        }

        _progressBar.Tick(string.Format(LoggingFormat, _numberOfMutantsRan, _mutantsToBeTested, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, _mutantsRuntimeErrorCount, RemainingTime()));
    }

    public void ReportFinalState()
    {
        _progressBar.Tick(string.Format(LoggingFormat, _numberOfMutantsRan, _mutantsToBeTested, _mutantsKilledCount, _mutantsSurvivedCount, _mutantsTimeoutCount, _mutantsRuntimeErrorCount, RemainingTime()));
        Dispose();

        var length = _mutantsToBeTested.ToString().Length;

        _console.WriteLine();
        _console.MarkupLine($"Killed:   [Magenta]{_mutantsKilledCount.ToString().PadLeft(length)}[/]");
        _console.MarkupLine($"Survived: [Magenta]{_mutantsSurvivedCount.ToString().PadLeft(length)}[/]");
        _console.MarkupLine($"Timeout:  [Magenta]{_mutantsTimeoutCount.ToString().PadLeft(length)}[/]");
        _console.MarkupLine($"Errors:  [Magenta]{_mutantsRuntimeErrorCount.ToString().PadLeft(length)}[/]");
    }

    public void ReportCoverageAnalysisStarted(int totalTests)
    {
        _coverageTotalTests = totalTests;
        _coverageAnalysisActive = true;
        CoverageTestsProcessed = 0;

        _spinnerTimer = new Timer(100)
        {
            AutoReset = true
        };
        _spinnerTimer.Elapsed += (sender, e) => WriteCoverageStatus(CoverageTestsProcessed);
        _spinnerTimer.Start();

        WriteCoverageStatus(0);
    }

    public void ReportCoverageAnalysisProgress(int testsProcessed)
    {
        if (!_coverageAnalysisActive)
        {
            return;
        }

        CoverageTestsProcessed = testsProcessed;
    }

    public void ReportCoverageAnalysisCompleted()
    {
        _coverageAnalysisActive = false;
        _spinnerTimer?.Stop();

        if (_coverageTotalTests > 0)
        {
            WriteCoverageStatus(_coverageTotalTests);
            System.Console.WriteLine();
        }
    }

    private void WriteCoverageStatus(int testsProcessed)
    {
        var percentage = _coverageTotalTests == 0 ? 0 : (int)((double)testsProcessed / _coverageTotalTests * 100);
        var spinner = _spinnerFrames[_spinnerIndex++ % _spinnerFrames.Length];

        System.Console.Write($"\r{spinner} Analyzing coverage [{testsProcessed}/{_coverageTotalTests}] {percentage}%");
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
                _spinnerTimer?.Dispose();
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
