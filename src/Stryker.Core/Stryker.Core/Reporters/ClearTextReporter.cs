using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The clear text reporter, prints a table with results.
    /// </summary>
    public class ClearTextReporter : IReporter
    {
        private readonly IChalk _chalk;
        private readonly StrykerOptions _options;

        public ClearTextReporter(StrykerOptions strykerOptions, IChalk chalk = null)
        {
            _options = strykerOptions;
            _chalk = chalk ?? new Chalk();
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
            // This reporter does not report during the testrun
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            // This reporter does not report during the testrun
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // This reporter does not report during the testrun
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var files = new List<FileLeaf>();

            FolderComposite rootFolder = null;

            reportComponent.DisplayFolder = (int _, IReadOnlyInputComponent current) =>
            {
                rootFolder ??= (FolderComposite)current;
            };

            reportComponent.DisplayFile = (int _, IReadOnlyInputComponent current) =>
            {
                var fileLeaf = (FileLeaf)current;

                files.Add((FileLeaf)current);
            };

            // print empty line for readability
            _chalk.Default($"{Environment.NewLine}{Environment.NewLine}All mutants have been tested, and your mutation score has been calculated{Environment.NewLine}");

            // start recursive invocation of handlers
            reportComponent.Display(0);

            var filePathLength = Math.Max(9, files.Max(f => f.RelativePathToProjectFile?.Length ?? 0) + 1);

            _chalk.Default($"┌─{new string('─', filePathLength)}┬──────────┬──────────┬───────────┬────────────┬──────────┬─────────┐{Environment.NewLine}");
            _chalk.Default($"│ File{new string(' ', filePathLength - 4)}│  % score │ # killed │ # timeout │ # survived │ # no cov │ # error │{Environment.NewLine}");
            _chalk.Default($"├─{new string('─', filePathLength)}┼──────────┼──────────┼───────────┼────────────┼──────────┼─────────┤{Environment.NewLine}");

            DisplayComponent(rootFolder, filePathLength);

            foreach (var file in files)
            {
                DisplayComponent(file, filePathLength);
            }

            _chalk.Default($"└─{new string('─', filePathLength)}┴──────────┴──────────┴───────────┴────────────┴──────────┴─────────┘{Environment.NewLine}");
        }

        private void DisplayComponent(ProjectComponent inputComponent, int filePathLength)
        {
            _chalk.Default($"│ {(inputComponent.RelativePathToProjectFile ?? "All files").PadRight(filePathLength)}│ ");

            var mutationScore = inputComponent.GetMutationScore();

            if (inputComponent is FileLeaf && inputComponent.IsComponentExcluded(_options.FilePatterns))
            {
                _chalk.DarkGray("Excluded");
            }
            else if (double.IsNaN(mutationScore))
            {
                _chalk.DarkGray("     N/A");
            }
            else
            {
                var scoreText = $"{mutationScore * 100:N2}".PadLeft(8);

                var checkHealth = inputComponent.CheckHealth(_options.Thresholds);
                if (checkHealth is Health.Good)
                {
                    _chalk.Green(scoreText);
                }
                else if (checkHealth is Health.Warning)
                {
                    _chalk.Yellow(scoreText);
                }
                else if (checkHealth is Health.Danger)
                {
                    _chalk.Red(scoreText);
                }
            }

            _chalk.Default($" │ {inputComponent.ReadOnlyMutants.Count(m => m.ResultStatus == MutantStatus.Killed),8}");
            _chalk.Default($" │ {inputComponent.ReadOnlyMutants.Count(m => m.ResultStatus == MutantStatus.Timeout),9}");
            _chalk.Default($" │ {inputComponent.TotalMutants.Count() - inputComponent.DetectedMutants.Count(),10}");
            _chalk.Default($" │ {inputComponent.ReadOnlyMutants.Count(m => m.ResultStatus == MutantStatus.NoCoverage),8}");
            _chalk.Default($" │ {inputComponent.ReadOnlyMutants.Count(m => m.ResultStatus == MutantStatus.CompileError),7}");
            _chalk.Default($" │{Environment.NewLine}");
        }
    }
}
