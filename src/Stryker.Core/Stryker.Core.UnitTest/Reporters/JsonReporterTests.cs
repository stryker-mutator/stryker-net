using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using static Stryker.Core.Reporters.JsonReporter;

namespace Stryker.Core.UnitTest.Reporters
{
    public class JsonReporterTests
    {
        [Fact]
        public void JsonReportComponent_ShouldGenerateFileTreeForJsonSerialization()
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = MutatorType.Arithmetic
            };

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant()
                    {
                        Id = 55,
                        ResultStatus = MutantStatus.Killed,
                        Mutation = mutation
                    }
                },
                SourceCode = "void M(){ int i = 0 + 8; }"
            });
            folder.Add(new FileLeaf()
            {
                Name = "SomeOtherFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant()
                    {
                        Id = 56,
                        ResultStatus = MutantStatus.Survived,
                        Mutation = mutation
                    }
                },
                SourceCode = "void M(){ int i = 0 + 8; }"
            });
            folder.Add(new FileLeaf()
            {
                Name = "SomeOtherFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant()
                    {
                        Id = 56,
                        ResultStatus = MutantStatus.Skipped,
                        Mutation = mutation
                    }
                },
                SourceCode = "void M(){ int i = 0 + 8; }"
            });

            var result = JsonReportComponent.FromProjectComponent(folder, new ThresholdOptions(80, 60, 0));

            ValidateJsonReportComponent(result, folder, "warning");
            ValidateJsonReportComponent(result.ChildResults.ElementAt(0), folder.Children.ElementAt(0), "ok", 1);
            ValidateJsonReportComponent(result.ChildResults.ElementAt(1), folder.Children.ElementAt(1), "danger", 1);
        }

        private void ValidateJsonReportComponent(JsonReportComponent jsonComponent, IReadOnlyInputComponent inputComponent, string health, int mutants = 0)
        {
            jsonComponent.Name.ShouldBe(inputComponent.Name);
            jsonComponent.Health.ShouldBe(health);
            jsonComponent.PossibleMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus != MutantStatus.BuildError).Count());
            jsonComponent.MutationScore.ShouldBe(inputComponent.GetMutationScore());
            jsonComponent.CompileErrors.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.BuildError).Count());
            jsonComponent.SurvivedMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Survived).Count());
            jsonComponent.SkippedMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Skipped).Count());
            jsonComponent.TimeoutMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Timeout).Count());
            jsonComponent.KilledMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Killed).Count());
            jsonComponent.TotalMutants.ShouldBe(inputComponent.TotalMutants.Count());
            jsonComponent.ThresholdHigh.ShouldBe(80);
            jsonComponent.ThresholdLow.ShouldBe(60);
            jsonComponent.ThresholdBreak.ShouldBe(0);

            if(inputComponent is FolderComposite folderComponent)
            {
                jsonComponent.Source.ShouldBe(null);
                jsonComponent.Mutants.ShouldBeEmpty();
                jsonComponent.ChildResults.Count.ShouldBe(folderComponent.Children.Count());
            }
            if(inputComponent is FileLeaf fileComponent)
            {
                jsonComponent.Source.ShouldBe(fileComponent.SourceCode);
                jsonComponent.Mutants.Count.ShouldBe(mutants);

                for (int i = 0; i < mutants; i++)
                {
                    jsonComponent.Mutants[i].Id.ShouldBe(fileComponent.Mutants.ToArray()[i].Id);
                    jsonComponent.Mutants[i].MutatorName.ShouldBe(fileComponent.Mutants.ToArray()[i].Mutation.DisplayName);
                    jsonComponent.Mutants[i].Replacement.ShouldBe(fileComponent.Mutants.ToArray()[i].Mutation.ReplacementNode.ToString());
                    jsonComponent.Mutants[i].Span.ShouldBe(
                        new[] 
                        {
                            fileComponent.Mutants.ToArray()[i].Mutation.OriginalNode.SpanStart,
                            fileComponent.Mutants.ToArray()[i].Mutation.OriginalNode.Span.End
                        });
                    jsonComponent.Mutants[i].Status.ShouldBe(fileComponent.Mutants.ToArray()[i].ResultStatus.ToString());
                }
            }
        }
    }
}
