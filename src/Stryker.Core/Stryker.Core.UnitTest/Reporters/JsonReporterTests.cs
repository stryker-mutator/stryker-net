using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Shouldly;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

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

            var result = JsonReporter.JsonReportComponent.FromProjectComponent(folder, new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60));
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
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

            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new JsonReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(folder);
            var reportPath = Path.Combine(options.BasePath, "StrykerOutput", "reports", $"mutation-report-{DateTime.Today.ToString("yyyy-mm-dd")}.json");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue();

            var reportObject = JsonConvert.DeserializeObject<JsonReporter.JsonReportComponent>(mockFileSystem.GetFile(reportPath).TextContents);
            reportObject.ThresholdHigh.ShouldBe(80);
            reportObject.ThresholdLow.ShouldBe(60);
            reportObject.ThresholdBreak.ShouldBe(0);

            ValidateJsonReportComponent(reportObject, folder, "warning");
            ValidateJsonReportComponent(reportObject.ChildResults.ElementAt(0), folder.Children.ElementAt(0), "ok", 1);
            ValidateJsonReportComponent(reportObject.ChildResults.ElementAt(1), folder.Children.ElementAt(1), "danger", 1);
        }

        private void ValidateJsonReportComponent(JsonReporter.JsonReportComponent jsonComponent, IReadOnlyInputComponent inputComponent, string health, int mutants = 0)
        {
            jsonComponent.Health.ShouldBe(health);
            jsonComponent.PossibleMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus != MutantStatus.BuildError).Count());
            jsonComponent.MutationScore.ShouldBe(inputComponent.GetMutationScore());
            jsonComponent.CompileErrors.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.BuildError).Count());
            jsonComponent.SurvivedMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Survived).Count());
            jsonComponent.SkippedMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Skipped).Count());
            jsonComponent.TimeoutMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Timeout).Count());
            jsonComponent.KilledMutants.ShouldBe(inputComponent.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Killed).Count());
            jsonComponent.TotalMutants.ShouldBe(inputComponent.TotalMutants.Count());

            if (inputComponent is FolderComposite folderComponent)
            {
                var componentNameToTest = folderComponent.Name is null ? null : folderComponent.RelativePath;
                jsonComponent.Name.ShouldBe(componentNameToTest);
                jsonComponent.Source.ShouldBe(null);
                jsonComponent.Mutants.ShouldBe(null);
                jsonComponent.ChildResults.Count.ShouldBe(folderComponent.Children.Count());
            }
            if (inputComponent is FileLeaf fileComponent)
            {
                jsonComponent.Name.ShouldBe(fileComponent.Name);
                jsonComponent.Source.ShouldBe(fileComponent.SourceCode);
                jsonComponent.Mutants.Count.ShouldBe(mutants);

                for (int i = 0; i < mutants; i++)
                {
                    jsonComponent.Mutants[i].Id.ShouldBe(fileComponent.Mutants.ToArray()[i].Id);
                    jsonComponent.Mutants[i].MutatorName.ShouldBe(fileComponent.Mutants.ToArray()[i].Mutation.DisplayName);
                    jsonComponent.Mutants[i].Replacement.ShouldBe(fileComponent.Mutants.ToArray()[i].Mutation.ReplacementNode.ToString());

                    var location = fileComponent.Mutants.ToArray()[i].Mutation.OriginalNode.SyntaxTree.GetLineSpan(fileComponent.Mutants.ToArray()[i].Mutation.OriginalNode.FullSpan);

                    jsonComponent.Mutants[i].Location.Start.Line.ShouldBe(location.StartLinePosition.Line + 1);
                    jsonComponent.Mutants[i].Location.Start.Column.ShouldBe(location.StartLinePosition.Character);
                    jsonComponent.Mutants[i].Location.End.Line.ShouldBe(location.EndLinePosition.Line + 1);
                    jsonComponent.Mutants[i].Location.End.Column.ShouldBe(location.EndLinePosition.Character);
                    jsonComponent.Mutants[i].Status.ShouldBe(fileComponent.Mutants.ToArray()[i].ResultStatus.ToString());
                }
            }
        }
    }
}
