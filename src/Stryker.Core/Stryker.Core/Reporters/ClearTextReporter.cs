using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress and end result.
    /// </summary>
    public class ClearTextReporter : IReporter
    {
        private const string ContinueLine = "│   ";
        private const string NoLine = "    ";
        private const string BranchLine = "├── ";
        private const string FinalBranchLine = "└── ";

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
            // setup display handlers
            reportComponent.DisplayFolder = (int depth, IReadOnlyInputComponent current) =>
            {
                // show depth
                var continuationLines = ParentContinuationLines(current);

                var stringBuilder = new StringBuilder();
                foreach (var item in continuationLines.SkipLast(1))
                {
                    stringBuilder.Append(item ? ContinueLine : NoLine);
                }

                var folderLines = string.Empty;
                if (continuationLines.Count > 0)
                {
                    folderLines = continuationLines.Last() ? BranchLine : FinalBranchLine;
                }

                _chalk.Default($"{stringBuilder}{folderLines}{current.Name}");
                DisplayComponent(current);
            };

            reportComponent.DisplayFile = (int depth, IReadOnlyInputComponent current) =>
            {
                // show depth
                var continuationLines = ParentContinuationLines(current);

                var stringBuilder = new StringBuilder();
                foreach (var item in continuationLines.SkipLast(1))
                {
                    stringBuilder.Append(item ? ContinueLine : NoLine);
                }

                _chalk.Default($"{stringBuilder}{(continuationLines.Last() ? BranchLine : FinalBranchLine)}{current.Name}");
                DisplayComponent(current);

                stringBuilder.Append(continuationLines.Last() ? ContinueLine : NoLine);

                var prefix = stringBuilder.ToString();

                foreach (var mutant in current.TotalMutants)
                {
                    var isLastMutant = current.TotalMutants.Last() == mutant;

                    _chalk.Default($"{prefix}{(isLastMutant ? FinalBranchLine : BranchLine)}");

                    switch (mutant.ResultStatus)
                    {
                        case MutantStatus.Killed:
                        case MutantStatus.Timeout:
                            _chalk.Green($"[{mutant.ResultStatus}]");
                            break;
                        case MutantStatus.NoCoverage:
                            _chalk.Yellow($"[{mutant.ResultStatus}]");
                            break;
                        default:
                            _chalk.Red($"[{mutant.ResultStatus}]");
                            break;
                    }

                    _chalk.Default($" {mutant.Mutation.DisplayName} on line {mutant.Line}{Environment.NewLine}");
                    _chalk.Default($"{prefix}{(isLastMutant ? NoLine : ContinueLine)}{BranchLine}[-] {mutant.Mutation.OriginalNode}{Environment.NewLine}");
                    _chalk.Default($"{prefix}{(isLastMutant ? NoLine : ContinueLine)}{FinalBranchLine}[+] {mutant.Mutation.ReplacementNode}{Environment.NewLine}");
                }
            };

            // print empty line for readability
            _chalk.Default($"{Environment.NewLine}{Environment.NewLine}All mutants have been tested, and your mutation score has been calculated{Environment.NewLine}");

            // start recursive invocation of handlers
            reportComponent.Display(1);
        }

        private static List<bool> ParentContinuationLines(IReadOnlyInputComponent current)
        {
            var continuationLines = new List<bool>();

            var node = (ProjectComponent)current;
            while (node.Parent != null)
            {
                continuationLines.Add(node.Parent.Children.Last() != node);

                node = node.Parent;
            }

            continuationLines.Reverse();

            return continuationLines;
        }

        private void DisplayComponent(IReadOnlyInputComponent inputComponent)
        {
            var mutationScore = inputComponent.GetMutationScore();
            // Convert the threshold integer values to decimal values

            _chalk.Default($" [{ inputComponent.DetectedMutants.Count()}/{ inputComponent.TotalMutants.Count()} ");

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
