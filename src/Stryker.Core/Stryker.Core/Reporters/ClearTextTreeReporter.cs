using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

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

        private readonly IStrykerOptions _options;
        private readonly TextWriter _consoleWriter;

        public ClearTextTreeReporter(IStrykerOptions strykerOptions, TextWriter consoleWriter = null)
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
            var rootFolderProcessed = false;

            // setup display handlers
            reportComponent.DisplayFolder = (IReadOnlyProjectComponent current) =>
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

                var name = Path.GetFileName(current.RelativePath);
                if (current.Parent == null && !rootFolderProcessed)
                {
                    name = "All files";
                    rootFolderProcessed = true;
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    _consoleWriter.Write($"{stringBuilder}{folderLines}{name}");
                    DisplayComponent(current);
                }
            };

            reportComponent.DisplayFile = (IReadOnlyProjectComponent current) =>
            {
                // show depth
                var continuationLines = ParentContinuationLines(current);

                var stringBuilder = new StringBuilder();
                foreach (var item in continuationLines.SkipLast(1))
                {
                    stringBuilder.Append(item ? ContinueLine : NoLine);
                }
                var name = Path.GetFileName(current.RelativePath);

                _consoleWriter.Write($"{stringBuilder}{(continuationLines.Last() ? BranchLine : FinalBranchLine)}{name}");
                DisplayComponent(current);

                stringBuilder.Append(continuationLines.Last() ? ContinueLine : NoLine);

                var prefix = stringBuilder.ToString();

                foreach (var mutant in current.TotalMutants)
                {
                    var isLastMutant = current.TotalMutants.Last() == mutant;

                    _consoleWriter.Write($"{prefix}{(isLastMutant ? FinalBranchLine : BranchLine)}");

                    switch (mutant.ResultStatus)
                    {
                        case MutantStatus.Killed:
                        case MutantStatus.Timeout:
                            _consoleWriter.Write(Output.Green($"[{mutant.ResultStatus}]"));
                            break;
                        case MutantStatus.NoCoverage:
                            _consoleWriter.Write(Output.Yellow($"[{mutant.ResultStatus}]"));
                            break;
                        default:
                            _consoleWriter.Write(Output.Red($"[{mutant.ResultStatus}]"));
                            break;
                    }

                    _consoleWriter.WriteLine($" {mutant.Mutation.DisplayName} on line {mutant.Line}");
                    _consoleWriter.WriteLine($"{prefix}{(isLastMutant ? NoLine : ContinueLine)}{BranchLine}[-] {mutant.Mutation.OriginalNode}");
                    _consoleWriter.WriteLine($"{prefix}{(isLastMutant ? NoLine : ContinueLine)}{FinalBranchLine}[+] {mutant.Mutation.ReplacementNode}");
                }
            };

            // print empty line for readability
            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine("All mutants have been tested, and your mutation score has been calculated");

            // start recursive invocation of handlers
            reportComponent.Display();
        }

        private static List<bool> ParentContinuationLines(IReadOnlyProjectComponent current)
        {
            var continuationLines = new List<bool>();

            var node = current;

            if (node.Parent != null)
            {
                while (node.Parent != null)
                {
                    continuationLines.Add(node.Parent.Children.Last().FullPath != node.FullPath);

                    node = node.Parent.ToReadOnlyInputComponent();
                }

                continuationLines.Reverse();
            }

            return continuationLines;
        }

        private void DisplayComponent(IReadOnlyProjectComponent inputComponent)
        {
            var mutationScore = inputComponent.GetMutationScore();

            // Convert the threshold integer values to decimal values
            _consoleWriter.Write($" [{ inputComponent.DetectedMutants.Count()}/{ inputComponent.TotalMutants.Count()} ");

            if (inputComponent.IsComponentExcluded(_options.FilePatterns))
            {
                _consoleWriter.Write(Output.Bright.Black("(Excluded)"));
            }
            else if (double.IsNaN(mutationScore))
            {
                _consoleWriter.Write(Output.Bright.Black("(N/A)"));
            }
            else
            {
                // print the score as a percentage
                string scoreText = string.Format("({0:P2})", mutationScore);
                if (inputComponent.CheckHealth(_options.Thresholds) is Health.Good)
                {
                    _consoleWriter.Write(Output.Green(scoreText));
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Warning)
                {
                    _consoleWriter.Write(Output.Yellow(scoreText));
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Danger)
                {
                    _consoleWriter.Write(Output.Red(scoreText));
                }
            }
            _consoleWriter.WriteLine("]");
        }
    }
}
