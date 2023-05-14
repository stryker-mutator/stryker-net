using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Spectre.Console;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The clear text tree reporter, prints a tree structure with results.
    /// </summary>
    public class ClearTextTreeReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IAnsiConsole _console;

        public ClearTextTreeReporter(StrykerOptions strykerOptions, IAnsiConsole console = null)
        {
            _options = strykerOptions;
            _console = console ?? AnsiConsole.Console;
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
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

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
        {
            Tree root = null;

            var stack = new Stack<IHasTreeNodes>();

            // setup display handlers
            reportComponent.DisplayFolder = (IReadOnlyProjectComponent current) =>
            {
                var name = Path.GetFileName(current.RelativePath);

                if (root is null)
                {
                    root = new Tree("All files" + DisplayComponent(current));
                    stack.Push(root);
                }
                else if (!string.IsNullOrWhiteSpace(name))
                {
                    stack.Push(stack.Peek().AddNode(name + DisplayComponent(current)));
                }
            };

            reportComponent.DisplayFile = (IReadOnlyProjectComponent current) =>
            {
                var name = Path.GetFileName(current.RelativePath);

                var fileNode = stack.Peek().AddNode(name + DisplayComponent(current));

                if (current.FullPath == current.Parent.Children.Last().FullPath)
                {
                    stack.Pop();
                }

                var totalMutants = current.TotalMutants();
                foreach (var mutant in totalMutants)
                {
                    var status = mutant.ResultStatus switch
                    {
                        MutantStatus.Killed or MutantStatus.Timeout => $"[Green][[{mutant.ResultStatus}]][/]",
                        MutantStatus.NoCoverage => $"[Yellow][[{mutant.ResultStatus}]][/]",
                        _ => $"[Red][[{mutant.ResultStatus}]][/]",
                    };

                    var mutantNode = fileNode.AddNode(status + $" {mutant.Mutation.DisplayName} on line {mutant.Line}");
                    mutantNode.AddNode(Markup.Escape($"[-] {mutant.Mutation.OriginalNode}"));
                    mutantNode.AddNode(Markup.Escape($"[+] {mutant.Mutation.ReplacementNode}"));
                }
            };

            // print empty line for readability
            _console.WriteLine();
            _console.WriteLine();
            _console.WriteLine("All mutants have been tested, and your mutation score has been calculated");

            // start recursive invocation of handlers
            reportComponent.Display();

            _console.Write(root);
        }

        private string DisplayComponent(IReadOnlyProjectComponent inputComponent)
        {
            var mutationScore = inputComponent.GetMutationScore();

            var stringBuilder = new StringBuilder();

            // Convert the threshold integer values to decimal values
            stringBuilder.Append($" [[{ inputComponent.DetectedMutants().Count()}/{ inputComponent.TotalMutants().Count()} ");

            if (inputComponent.IsComponentExcluded(_options.Mutate))
            {
                stringBuilder.Append("[Gray](Excluded)[/]");
            }
            else if (double.IsNaN(mutationScore))
            {
                stringBuilder.Append("[Gray](N/A)[/]");
            }
            else
            {
                // print the score as a percentage
                var scoreText = string.Format("({0:P2})", mutationScore);
                if (inputComponent.CheckHealth(_options.Thresholds) is Health.Good)
                {
                    stringBuilder.Append($"[Green]{scoreText}[/]");
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Warning)
                {
                    stringBuilder.Append($"[Yellow]{scoreText}[/]");
                }
                else if (inputComponent.CheckHealth(_options.Thresholds) is Health.Danger)
                {
                    stringBuilder.Append($"[Red]{scoreText}[/]");
                }
            }

            stringBuilder.Append("]]");

            return stringBuilder.ToString();
        }
    }
}
