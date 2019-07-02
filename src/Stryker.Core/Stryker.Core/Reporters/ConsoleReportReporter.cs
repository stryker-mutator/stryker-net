﻿using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress and end result.
    /// </summary>
    public class ConsoleReportReporter : IReporter
    {
        private readonly IChalk _chalk;
        private readonly StrykerOptions _options;

        public ConsoleReportReporter(StrykerOptions strykerOptions, IChalk chalk = null)
        {
            _options = strykerOptions;
            _chalk = chalk ?? new Chalk();
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<Mutant> mutantsToBeTested)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent)
        {
            // setup display handlers
            inputComponent.DisplayFolder = (int depth, IReadOnlyInputComponent current) =>
            {
                // show depth
                _chalk.Default($"{new string('-', depth)} {Path.DirectorySeparatorChar}{Path.GetFileName(current.Name)} ");
                DisplayComponent(current);
            };

            inputComponent.DisplayFile = (int depth, IReadOnlyInputComponent current) =>
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
                    else
                    {
                        _chalk.Red($"[{mutant.ResultStatus}] ");
                    }
                    _chalk.Default($"{mutant.Mutation.DisplayName} on line {mutant.Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line + 1}: '{mutant.Mutation.OriginalNode}' ==> '{mutant.Mutation.ReplacementNode}'{Environment.NewLine}");
                }
            };

            // print empty line for readability
            _chalk.Default($"{Environment.NewLine}{Environment.NewLine}All mutants have been tested, and your mutation score has been calculated{Environment.NewLine}");
            // start recursive invocation of handlers
            inputComponent.Display(1);
        }

        private void DisplayComponent(IReadOnlyInputComponent inputComponent)
        {
            var score = inputComponent.GetMutationScore();
            // Convert the threshold integer values to decimal values

            _chalk.Default($"[{ inputComponent.DetectedMutants.Count()}/{ inputComponent.TotalMutants.Count()} ");
            if (inputComponent.IsExcluded)
            {
                _chalk.DarkGray($"(Excluded)");
            }
            else if (!score.HasValue)
            {
                _chalk.DarkGray($"(- %)");
            }
            else
            {
                // print the score as a percentage
                string scoreText = $"({ (score.Value / 100).ToString("p", CultureInfo.InvariantCulture)})";
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
