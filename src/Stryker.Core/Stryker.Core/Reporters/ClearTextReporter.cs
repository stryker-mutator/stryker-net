using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters;

/// <summary>
/// The clear text reporter, prints a table with results.
/// </summary>
public class ClearTextReporter : IReporter
{
    private readonly StrykerOptions _options;
    private readonly IAnsiConsole _console;

    public ClearTextReporter(StrykerOptions strykerOptions, IAnsiConsole console = null)
    {
        _options = strykerOptions;
        _console = console ?? AnsiConsole.Console;
    }

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
    {
        // This reporter does not report during the testrun
    }

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
    {
        // This reporter does not report during the testrun
    }

    public void OnMutantTested(IReadOnlyMutant result)
    {
        // This reporter does not report during the testrun
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
    {
        var files = reportComponent.GetAllFiles();
        if (files.Any())
        {
            // print empty line for readability
            _console.WriteLine();
            _console.WriteLine();
            _console.WriteLine("All mutants have been tested, and your mutation score has been calculated");

            var table = new Table()
                .RoundedBorder()
                .AddColumn("File", c => c.NoWrap())
                .AddColumn("% score", c => c.Alignment(Justify.Right).NoWrap())
                .AddColumn("# killed", c => c.Alignment(Justify.Right).NoWrap())
                .AddColumn("# timeout", c => c.Alignment(Justify.Right).NoWrap())
                .AddColumn("# survived", c => c.Alignment(Justify.Right).NoWrap())
                .AddColumn("# no cov", c => c.Alignment(Justify.Right).NoWrap())
                .AddColumn("# error", c => c.Alignment(Justify.Right).NoWrap());

            DisplayComponent(reportComponent, table);

            foreach (var file in files)
            {
                DisplayComponent(file, table);
            }

            _console.Write(table);
        }
    }

    private void DisplayComponent(IReadOnlyProjectComponent inputComponent, Table table)
    {
        var columns = new List<IRenderable>
        {
            new Text(inputComponent.RelativePath ?? "All files")
        };

        var mutationScore = inputComponent.GetMutationScore();

        if (inputComponent.IsComponentExcluded(_options.Mutate))
        {
            columns.Add(new Markup("[Gray]Excluded[/]"));
        }
        else if (double.IsNaN(mutationScore))
        {
            columns.Add(new Markup("[Gray]N/A[/]"));
        }
        else
        {
            var scoreText = $"{mutationScore * 100:N2}";

            var checkHealth = inputComponent.CheckHealth(_options.Thresholds);
            if (checkHealth is Health.Good)
            {
                columns.Add(new Markup($"[Green]{scoreText}[/]"));
            }
            else if (checkHealth is Health.Warning)
            {
                columns.Add(new Markup($"[Yellow]{scoreText}[/]"));
            }
            else if (checkHealth is Health.Danger)
            {
                columns.Add(new Markup($"[Red]{scoreText}[/]"));
            }
        }

        var mutants = inputComponent.Mutants.ToList();

        columns.Add(new Text(mutants.Count(m => m.ResultStatus == MutantStatus.Killed).ToString()));
        columns.Add(new Text(mutants.Count(m => m.ResultStatus == MutantStatus.Timeout).ToString()));
        columns.Add(new Text((inputComponent.TotalMutants().Count() - inputComponent.DetectedMutants().Count()).ToString()));
        columns.Add(new Text(mutants.Count(m => m.ResultStatus == MutantStatus.NoCoverage).ToString()));
        columns.Add(new Text(mutants.Count(m => m.ResultStatus == MutantStatus.CompileError).ToString()));

        table.AddRow(columns);
    }
}
