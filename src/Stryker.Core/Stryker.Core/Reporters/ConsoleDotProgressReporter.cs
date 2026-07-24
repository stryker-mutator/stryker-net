using System.Collections.Generic;
using Spectre.Console;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;

namespace Stryker.Core.Reporters;

/// <summary>
/// The default reporter, prints a simple progress by printing dots
/// </summary>
public class ConsoleDotProgressReporter : IReporter
{
    private readonly IAnsiConsole _console;

    public ConsoleDotProgressReporter(IAnsiConsole console = null) => _console = console ?? AnsiConsole.Console;

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
        // don't report on mutant creation
    }

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
    {
        // nothing to initialize
    }

    public void OnMutantTested(IReadOnlyMutant result)
    {
        switch (result.ResultStatus)
        {
            case MutantStatus.Killed:
                _console.Write(".");
                break;
            case MutantStatus.Survived:
                _console.Markup("[Red]S[/]");
                break;
            case MutantStatus.Timeout:
                _console.Write("T");
                break;
            case MutantStatus.RuntimeError:
                _console.Markup("[Yellow]E[/]");
                break;
        }
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo) => _console.WriteLine();

    public void OnCoverageAnalysisStarted(int totalTests) => _console.WriteLine("Analyzing coverage...");

    public void OnCoverageAnalysisProgress(int testsCompleted, int totalTests)
    {
        // don't report per-test during coverage analysis
    }

    public void OnCoverageAnalysisCompleted() => _console.WriteLine();
}
