using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The clear text tree reporter, prints a tree structure with results.
    /// </summary>
    public class ClearTextTreeReporter : IReporter
    {
        private const string ContinueLine = "│   ";
        private const string NoLine = "    ";
        private const string BranchLine = "├── ";
        private const string FinalBranchLine = "└── ";

        private readonly StrykerOptions _options;
        private readonly TextWriter _output;

        public ClearTextTreeReporter(StrykerOptions strykerOptions, TextWriter output = null)
        {
            _options = strykerOptions;
            _output = output ?? Console.Out;
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
            var rootFolderProcessed = false;

            // setup display handlers
            reportComponent.DisplayFolder = (int _, IReadOnlyInputComponent current) =>
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

                var name = current.Name;
                if (name == null && !rootFolderProcessed)
                {
                    name = "All files";
                    rootFolderProcessed = true;
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    _output.Write($"{stringBuilder}{folderLines}{name}");
                    DisplayComponent(current);
                }
            };

            reportComponent.DisplayFile = (int _, IReadOnlyInputComponent current) =>
            {
                // show depth
                var continuationLines = ParentContinuationLines(current);

                var stringBuilder = new StringBuilder();
                foreach (var item in continuationLines.SkipLast(1))
                {
                    stringBuilder.Append(item ? ContinueLine : NoLine);
                }

                _output.Write($"{stringBuilder}{(continuationLines.Last() ? BranchLine : FinalBranchLine)}{current.Name}");
                DisplayComponent(current);

                stringBuilder.Append(continuationLines.Last() ? ContinueLine : NoLine);

                var prefix = stringBuilder.ToString();

                foreach (var mutant in current.TotalMutants)
                {
                    var isLastMutant = current.TotalMutants.Last() == mutant;

                    _output.Write($"{prefix}{(isLastMutant ? FinalBranchLine : BranchLine)}");

                    switch (mutant.ResultStatus)
                    {
                        case MutantStatus.Killed:
                        case MutantStatus.Timeout:
                            _output.Write(Output.Green($"[{mutant.ResultStatus}]"));
                            break;
                        case MutantStatus.NoCoverage:
                            _output.Write(Output.Yellow($"[{mutant.ResultStatus}]"));
                            break;
                        default:
                            _output.Write(Output.Red($"[{mutant.ResultStatus}]"));
                            break;
                    }

                    _output.WriteLine($" {mutant.Mutation.DisplayName} on line {mutant.Line}");
                    _output.WriteLine($"{prefix}{(isLastMutant ? NoLine : ContinueLine)}{BranchLine}[-] {mutant.Mutation.OriginalNode}");
                    _output.WriteLine($"{prefix}{(isLastMutant ? NoLine : ContinueLine)}{FinalBranchLine}[+] {mutant.Mutation.ReplacementNode}");
                }
            };

            // print empty line for readability
            _output.WriteLine();
            _output.WriteLine();
            _output.WriteLine("All mutants have been tested, and your mutation score has been calculated");

            // start recursive invocation of handlers
            reportComponent.Display(1);
        }

        private static List<bool> ParentContinuationLines(IReadOnlyInputComponent current)
        {
            var continuationLines = new List<bool>();

            var node = (ProjectComponent)current;

            if (node.Parent != null)
            {
                var isRootFile = node.RelativePath == node.RelativePathToProjectFile;
                if (isRootFile)
                {
                    continuationLines.Add(true);
                }
                else
                {
                    while (node.Parent != null)
                    {
                        continuationLines.Add(node.Parent.Children.Last() != node);

                        node = node.Parent;
                    }

                    continuationLines.Reverse();
                }
            }

            return continuationLines;
        }

        private void DisplayComponent(IReadOnlyInputComponent inputComponent)
        {
            var mutationScore = inputComponent.GetMutationScore();

            // Convert the threshold integer values to decimal values
            _output.Write($" [{ inputComponent.DetectedMutants.Count()}/{ inputComponent.TotalMutants.Count()} ");

            if (inputComponent is ProjectComponent projectComponent && projectComponent.FullPath != null && projectComponent.IsComponentExcluded(_options.FilePatterns))
            {
                _output.Write(Output.BrightBlack("(Excluded)"));
            }
            else if (double.IsNaN(mutationScore))
            {
                _output.Write(Output.BrightBlack("(N/A)"));
            }
            else
            {
                // print the score as a percentage
                string scoreText = string.Format("({0:P2})", mutationScore);
                if (inputComponent.CheckHealth(_options.Thresholds) is Health.Good)
                {
                    _output.Write(Output.Green(scoreText));
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Warning)
                {
                    _output.Write(Output.Yellow(scoreText));
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Danger)
                {
                    _output.Write(Output.Red(scoreText));
                }
            }
            _output.WriteLine("]");
        }
    }
}
