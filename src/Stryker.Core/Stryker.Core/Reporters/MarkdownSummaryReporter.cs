using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Grynwald.MarkdownGenerator;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using static FSharp.Compiler.AbstractIL.IL.ILExceptionClause;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// Markdown result table reporter.
    /// </summary>
    public class MarkdownSummaryReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IAnsiConsole _console;
        private readonly IFileSystem _fileSystem;

        public MarkdownSummaryReporter(StrykerOptions strykerOptions, IFileSystem fileSystem = null, IAnsiConsole console = null)
        {
            _options = strykerOptions;
            _console = console ?? AnsiConsole.Console;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
        {
            // This reporter does not report during the testrun
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
        {
            // This reporter does not report during the testrun
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // This reporter does not report during the testrun
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
            var files = reportComponent.GetAllFiles();
            if (files.Any())
            {
                var filename = _options.ReportFileName + ".md";
                var reportPath = Path.Combine(_options.ReportPath, filename);
                var reportUri = "file://" + reportPath.Replace("\\", "/");

                GenerateMarkdownReport(_options, reportPath, files, reportComponent.GetMutationScore());

                _console.WriteLine();
                _console.MarkupLine("[Green]Your Markdown summary has been generated at:[/]");

                if (_console.Profile.Capabilities.Links)
                {
                    // We must print the report path as the link text because on some terminals links might be supported but not actually clickable: https://github.com/spectreconsole/spectre.console/issues/764
                    _console.MarkupLineInterpolated($"[Green][link={reportUri}]{reportPath}[/][/]");
                }
                else
                {
                    _console.MarkupLineInterpolated($"[Green]{reportUri}[/]");
                }
            }
        }

        private void GenerateMarkdownReport(StrykerOptions options, string reportPath, IEnumerable<IFileLeaf> files, double mutationScore)
        {
            if (!files.Any())
            {
                return;
            }

            var mdSummaryDocument = new MdDocument();
            mdSummaryDocument.Root.Add(new MdHeading(1, "Mutation Testing Summary"));

            var mdSummary = new MdTable(new MdTableRow(new[] { "File", "% score", "# killed", "# timeout", "# survived", "# no cov", "# error" }));

            foreach (var file in files)
            {
                mdSummary.Add(GenerateFileData(file));
            }

            mdSummaryDocument.Root.Add(mdSummary);
            mdSummaryDocument.Root.Add(new MdHeading(2, new MdTextSpan($"The final mutation score is {mutationScore * 100:N2}%")));
            mdSummaryDocument.Root.Add(new MdHeading(3, new MdEmphasisSpan($"Coverage Thresholds: high:{_options.Thresholds.High} low:{_options.Thresholds.Low} break:{_options.Thresholds.Break}")));

            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
            using var mdFile = _fileSystem.File.Create(reportPath);
            mdSummaryDocument.Save(mdFile);
        }

        private MdTableRow GenerateFileData(IFileLeaf fileScores)
        {
            var values = new List<string>();

            values.Add(fileScores.RelativePath ?? "All files");

            var mutationScore = fileScores.GetMutationScore();

            if (fileScores.IsComponentExcluded(_options.Mutate))
            {
                values.Add("Excluded");
            }
            else if (double.IsNaN(mutationScore))
            {
                values.Add("N/A");
            }
            else
            {
                values.Add($"{mutationScore * 100:N2}");
            }

            var mutants = fileScores.Mutants.ToList();

            values.Add(mutants.Count(m => m.ResultStatus == MutantStatus.Killed).ToString());
            values.Add(mutants.Count(m => m.ResultStatus == MutantStatus.Timeout).ToString());
            values.Add((fileScores.TotalMutants().Count() - fileScores.DetectedMutants().Count()).ToString());
            values.Add(mutants.Count(m => m.ResultStatus == MutantStatus.NoCoverage).ToString());
            values.Add(mutants.Count(m => m.ResultStatus == MutantStatus.CompileError).ToString());

            return new MdTableRow(values);
        }
    }
}
