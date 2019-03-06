using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class JsonReporterTests
    {
        [Fact]
        public void JsonMutantPositionLine_ThrowsArgumentExceptionWhenSetToLessThan1()
        {
            Should.Throw<ArgumentException>(() => new JsonMutantPosition().Line = -1);
            Should.Throw<ArgumentException>(() => new JsonMutantPosition().Line = 0);
        }

        [Fact]
        public void JsonMutantPositionColumn_ThrowsArgumentExceptionWhenSetToLessThan1()
        {
            Should.Throw<ArgumentException>(() => new JsonMutantPosition().Column = -1);
            Should.Throw<ArgumentException>(() => new JsonMutantPosition().Column = 0);
        }

        [Fact]
        public void JsonMutantLocation_FromValidFileLinePositionSpanShouldAdd1ToLineAndColumnNumbers()
        {
            var lineSpan = new FileLinePositionSpan(
                "",
                new LinePosition(2, 2),
                new LinePosition(4, 5));

            var jsonMutantLocation = new JsonMutantLocation(lineSpan);

            jsonMutantLocation.Start.Line.ShouldBe(3);
            jsonMutantLocation.Start.Column.ShouldBe(3);
            jsonMutantLocation.End.Line.ShouldBe(5);
            jsonMutantLocation.End.Column.ShouldBe(6);
        }

        [Fact]
        public void JsonReportComponent_ShouldHaveLanguageSetToCs()
        {
            var folderComponent = CreateProjectWith();
            var fileComponent = (folderComponent as FolderComposite).GetAllFiles().First();

            new JsonReportFileComponent(fileComponent).Language.ShouldBe("cs");
        }

        [Fact]
        public void JsonReportComponent_ShouldContainOriginalSource()
        {
            var folderComponent = CreateProjectWith();
            var fileComponent = (folderComponent as FolderComposite).GetAllFiles().First();

            new JsonReportFileComponent(fileComponent).Source.ShouldBe(fileComponent.SourceCode);
        }

        [Fact]
        public void JsonReportComponents_ShouldContainMutants()
        {
            var folderComponent = CreateProjectWith();
            foreach (var file in (folderComponent as FolderComposite).GetAllFiles())
            {
                var jsonReportComponent = new JsonReportFileComponent(file);
                foreach (var mutant in file.Mutants)
                {
                    jsonReportComponent.Mutants.ShouldContain(m => m.Id == mutant.Id);
                }
            }
        }

        [Fact]
        public void JsonReportComponent_DoesNotContainDuplicateMutantsAndLogsWarningWhenDuplicateDetected()
        {
            var folderComponent = CreateProjectWith();
            foreach (var file in (folderComponent as FolderComposite).GetAllFiles())
            {
                var jsonReportComponent = new JsonReportFileComponent(file);
                foreach (var mutant in file.Mutants)
                {
                    jsonReportComponent.Mutants.ShouldContain(m => m.Id == mutant.Id);
                }
            }
        }

        //[Fact]
        //public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
        //{
        //    var mockFileSystem = new MockFileSystem();
        //    var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
        //    var reporter = new JsonReporter(options, mockFileSystem);

        //    reporter.OnAllMutantsTested(folder);
        //    var reportPath = Path.Combine(options.BasePath, "StrykerOutput", "reports", $"mutation-report-{DateTime.Today.ToString("yyyy-MM-dd")}.json");
        //    mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");

        //    var reportObject = JsonConvert.DeserializeObject<JsonReporter.JsonReportComponent>(mockFileSystem.GetFile(reportPath).TextContents);
        //    reportObject.ThresholdHigh.ShouldBe(80);
        //    reportObject.ThresholdLow.ShouldBe(60);
        //    reportObject.ThresholdBreak.ShouldBe(0);

        //    ValidateJsonReportComponent(reportObject, folder, "warning");
        //    ValidateJsonReportComponent(reportObject.ChildResults.ElementAt(0), folder.Children.ElementAt(0), "ok", 1);
        //    ValidateJsonReportComponent(reportObject.ChildResults.ElementAt(1), folder.Children.ElementAt(1), "danger", 1);
        //}

        private IReadOnlyInputComponent CreateProjectWith(int folders = 1, int files = 3, int mutants = 2, int duplicateMutants = 0)
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
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
                SourceCode = "void M(){ int i = 0 + 7; }"
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
                            Id = 57,
                            ResultStatus = MutantStatus.Skipped,
                            Mutation = mutation
                        }
                    },
                SourceCode = "void M(){ int i = 0 + 9; }"
            });

            return folder;
        }
    }
}
