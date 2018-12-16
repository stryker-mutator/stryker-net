using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
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
        private IChalk _chalk { get; set; }
        private StrykerOptions _options { get; }

        public ConsoleReportReporter(StrykerOptions strykerOptions, IChalk chalk = null)
        {
            _options = strykerOptions;
            _chalk = chalk ?? new Chalk();
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent)
        {
            // print empty line for readability
            Console.WriteLine("");

            _chalk.DarkGray($"{inputComponent.ExcludedFiles.Count} have been excluded. For these files there are no mutants created. {Environment.NewLine}");
            foreach (var excludedFile in inputComponent.ExcludedFiles)
                _chalk.DarkGray($"Excluded file: {excludedFile.FullPath} {Environment.NewLine}");

            _chalk.Default($"{inputComponent.TotalMutants.Count()} mutants have been created. Each mutant will now be tested, this could take a while. {Environment.NewLine}");
            
            // print empty line for readability
            Console.WriteLine("");
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
            decimal thresholdHigh = _options.ThresholdOptions.ThresholdHigh;
            decimal thresholdLow = _options.ThresholdOptions.ThresholdLow;
            decimal thresholdBreak = _options.ThresholdOptions.ThresholdBreak;

            _chalk.Default($"[{ inputComponent.DetectedMutants.Count()}/{ inputComponent.TotalMutants.Count()} ");
            if (!score.HasValue)
            {
                _chalk.DarkGray($"(- %)");
            }
            else
            {
                // print the score as a percentage
                string scoreText = $"({ (score.Value / 100).ToString("p", CultureInfo.InvariantCulture)})";
                if (score > thresholdHigh)
                {
                    _chalk.Green(scoreText);
                }
                else if (score > thresholdLow)
                {
                    _chalk.Yellow(scoreText);
                }
                else if (score <= thresholdLow)
                {
                    _chalk.Red(scoreText);
                }
            }
            _chalk.Default($"]{Environment.NewLine}");
        }
    }
}
