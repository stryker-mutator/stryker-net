using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The clear text reporter, prints a table with results.
    /// </summary>
    public class ClearTextReporter : IReporter
    {
        private readonly IStrykerOptions _options;
        private readonly TextWriter _consoleWriter;

        public ClearTextReporter(IStrykerOptions strykerOptions, TextWriter consoleWriter = null)
        {
            _options = strykerOptions;
            _consoleWriter = consoleWriter ?? Console.Out;
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
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

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
            var files = new List<ReadOnlyFileLeaf>();

            ReadOnlyFolderComposite rootFolder = null;

            reportComponent.DisplayFolder = (IReadOnlyProjectComponent current) =>
            {
                rootFolder ??= (ReadOnlyFolderComposite)current;
            };

            reportComponent.DisplayFile = (IReadOnlyProjectComponent current) =>
            {
                files.Add((ReadOnlyFileLeaf)current);
            };

            // print empty line for readability
            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine("All mutants have been tested, and your mutation score has been calculated");

            // start recursive invocation of handlers
            reportComponent.Display();

            var filePathLength = Math.Max(9, files.Max(f => f.RelativePath?.Length ?? 0) + 1);

            _consoleWriter.WriteLine($"┌─{new string('─', filePathLength)}┬──────────┬──────────┬───────────┬────────────┬──────────┬─────────┐");
            _consoleWriter.WriteLine($"│ File{new string(' ', filePathLength - 4)}│  % score │ # killed │ # timeout │ # survived │ # no cov │ # error │");
            _consoleWriter.WriteLine($"├─{new string('─', filePathLength)}┼──────────┼──────────┼───────────┼────────────┼──────────┼─────────┤");

            DisplayComponent(rootFolder, filePathLength);

            foreach (var file in files)
            {
                DisplayComponent(file, filePathLength);
            }

            _consoleWriter.WriteLine($"└─{new string('─', filePathLength)}┴──────────┴──────────┴───────────┴────────────┴──────────┴─────────┘");
        }

        private void DisplayComponent(IReadOnlyProjectComponent inputComponent, int filePathLength)
        {
            _consoleWriter.Write($"│ {(inputComponent.RelativePath ?? "All files").PadRight(filePathLength)}│ ");

            var mutationScore = inputComponent.GetMutationScore();

            if (inputComponent is ReadOnlyFileLeaf && inputComponent.IsComponentExcluded(_options.FilePatterns))
            {
                _consoleWriter.Write(Output.Bright.Black("Excluded"));
            }
            else if (double.IsNaN(mutationScore))
            {
                _consoleWriter.Write(Output.Bright.Black("     N/A"));
            }
            else
            {
                var scoreText = $"{mutationScore * 100:N2}".PadLeft(8);

                var checkHealth = inputComponent.CheckHealth(_options.Thresholds);
                if (checkHealth is Health.Good)
                {
                    _consoleWriter.Write(Output.Green(scoreText));
                }
                else if (checkHealth is Health.Warning)
                {
                    _consoleWriter.Write(Output.Yellow(scoreText));
                }
                else if (checkHealth is Health.Danger)
                {
                    _consoleWriter.Write(Output.Red(scoreText));
                }
            }

            _consoleWriter.Write($" │ {inputComponent.Mutants.Count(m => m.ResultStatus == MutantStatus.Killed),8}");
            _consoleWriter.Write($" │ {inputComponent.Mutants.Count(m => m.ResultStatus == MutantStatus.Timeout),9}");
            _consoleWriter.Write($" │ {inputComponent.TotalMutants.Count() - inputComponent.DetectedMutants.Count(),10}");
            _consoleWriter.Write($" │ {inputComponent.Mutants.Count(m => m.ResultStatus == MutantStatus.NoCoverage),8}");
            _consoleWriter.Write($" │ {inputComponent.Mutants.Count(m => m.ResultStatus == MutantStatus.CompileError),7}");
            _consoleWriter.WriteLine($" │");
        }
    }
}
