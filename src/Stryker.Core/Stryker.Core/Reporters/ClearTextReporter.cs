using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress and end result.
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

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
        {
            // This reporter does not report during the testrun
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // This reporter does not report during the testrun
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            // setup display handlers
            reportComponent.DisplayFolder = (int depth, IReadOnlyInputComponent current) =>
            {
                // show depth
                _chalk.Default($"{new string('-', depth)} {Path.DirectorySeparatorChar}{Path.GetFileName(current.Name)} ");
                DisplayComponent(current);
            };

            reportComponent.DisplayFile = (int depth, IReadOnlyInputComponent current) =>
            {
                // show depth
                _chalk.Default($"{new string('-', depth)} {current.Name} ");
                DisplayComponent(current);
                foreach (var mutant in current.TotalMutants)
                {
                    if (mutant.ResultStatus == MutantStatus.Killed ||
                    mutant.ResultStatus == MutantStatus.Timeout)
                    {
                        _chalk.Green($"[{mutant.ResultStatus}] ");
                    }
                    else if (mutant.ResultStatus == MutantStatus.NoCoverage)
                    {
                        _chalk.Yellow($"[{mutant.ResultStatus}] ");
                    }
                    else
                    {
                        _chalk.Red($"[{mutant.ResultStatus}] ");
                    }
                    _chalk.Default(mutant.LongName + Environment.NewLine);
                }
            };

            // print empty line for readability
            _chalk.Default($"{Environment.NewLine}{Environment.NewLine}All mutants have been tested, and your mutation score has been calculated{Environment.NewLine}");
            // start recursive invocation of handlers
            reportComponent.Display(1);
        }

        private void DisplayComponent(IReadOnlyInputComponent inputComponent)
        {
            var mutationScore = inputComponent.GetMutationScore();
            // Convert the threshold integer values to decimal values

            _chalk.Default($"[{ inputComponent.DetectedMutants.Count()}/{ inputComponent.TotalMutants.Count()} ");

            if (inputComponent is ProjectComponent projectComponent && projectComponent.IsComponentExcluded(_options.FilePatterns))
            {
                _chalk.DarkGray($"(Excluded)");
            }
            else if (double.IsNaN(mutationScore))
            {
                _chalk.DarkGray($"(N/A)");
            }
            else
            {
                // print the score as a percentage
                string scoreText = string.Format("({0:P2})", mutationScore);
                if (inputComponent.CheckHealth(_options.Thresholds) is Health.Good)
                {
                    _chalk.Green(scoreText);
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Warning)
                {
                    _chalk.Yellow(scoreText);
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Danger)
                {
                    _chalk.Red(scoreText);
                }
            }
            _chalk.Default($"]{Environment.NewLine}");
        }
    }
}
