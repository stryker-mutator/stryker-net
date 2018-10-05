using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Testing;
using Stryker.Core.Options;
using Stryker.Core.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress and end result.
    /// </summary>
    public class ConsoleReportReporter : IReporter
    {
        private IChalk _chalk { get; set; }
        private StrykerOptions options { get; }
        private ILogger _logger { get; set; }

        public ConsoleReportReporter(StrykerOptions strykerOptions, IChalk chalk = null)
        {
            options =  strykerOptions;
            _chalk = chalk ?? new Chalk();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ConsoleReportReporter>();
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent)
        {
            // print empty line for readability
            Console.WriteLine("");

            _chalk.Default($"{inputComponent.TotalMutants.Count()} mutants have been created. Each mutant will now be tested, this could take a while. {Environment.NewLine}");

            // print empty line for readability
            Console.WriteLine("");
        }

        public void OnMutantTested(IReadOnlyMutant result) { }

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent)
        {
            // setup display handlers
            inputComponent.DisplayFolder = (int depth, IReadOnlyInputComponent current) =>
            {
                // show depth
                _chalk.Default($"{new String('-', depth)} {Path.DirectorySeparatorChar}{Path.GetFileName(current.Name)} ");
                DisplayComponent(current);
            };

            inputComponent.DisplayFile = (int depth, IReadOnlyInputComponent current) =>
            {
                // show depth
                _chalk.Default($"{new String('-', depth)} {current.Name} ");
                DisplayComponent(current);
                foreach (var mutant in current.TotalMutants)
                {
                    if (mutant.ResultStatus == MutantStatus.Killed ||
                    mutant.ResultStatus == MutantStatus.Timeout ||
                    mutant.ResultStatus == MutantStatus.RuntimeError) {
                        _chalk.Green($"[{mutant.ResultStatus}] ");
                    } else
                    {
                        _chalk.Red($"[{mutant.ResultStatus}] ");
                    }
                    _chalk.Default($"{mutant.Mutation.DisplayName} on line {mutant.Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line}: '{mutant.Mutation.OriginalNode}' ==> '{mutant.Mutation.ReplacementNode}'{Environment.NewLine}");
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
            decimal thresholdHigh = (decimal) this.options.ThresholdOptions.ThresholdHigh/100;
            decimal thresholdLow = (decimal) this.options.ThresholdOptions.ThresholdLow/100;
            decimal thresholdBreak = (decimal) this.options.ThresholdOptions.ThresholdBreak/100;

            _chalk.Default($"[{ inputComponent.DetectedMutants.Count()}/{ inputComponent.TotalMutants.Count()} ");
            if (!score.HasValue)
            {
                _chalk.DarkGray($"(- %)");
            }
            else
            {
                // print the score as a percentage
                string scoreText = $"({ score.Value.ToString("P", CultureInfo.InvariantCulture)})";
                if (score > thresholdHigh)
                {
                    _chalk.Green(scoreText);
                }
                else if (score > thresholdLow)
                {
                    _chalk.Yellow(scoreText);
                } 
                else if (score <= thresholdLow && score >= thresholdBreak)
                {
                    _chalk.Red(scoreText);
                } 
                else 
                {
                    // Threshold break value has been reached, set a non-zero exit code
                    _logger.LogError("Final mutation score " 
                        + score + "under breaking threshold " 
                        + this.options.ThresholdOptions.ThresholdBreak 
                        + " , setting exit code to 1 (failure)");
                    _logger.LogInformation("Improve mutation score or set '--thresholds-break' lower to prevent this error in the future");
                    Environment.ExitCode = 1;
                }
            }
            _chalk.Default($"]{Environment.NewLine}");
        }

        public void OnInitialisationStarted() { }

        public void OnInitialBuildStarted() { }

        public void OnInitialTestRunStarted() { }

        public void OnInitialisationDone() { }
    }
}
