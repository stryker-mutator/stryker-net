using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
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
        public JsonReporterTests()
        {
            ApplicationLogging.ConfigureLogger(new LogOptions(Serilog.Events.LogEventLevel.Fatal, false, null));
            ApplicationLogging.LoggerFactory.CreateLogger<JsonReporterTests>();
        }

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
        public void JsonReportFileComponent_ShouldHaveLanguageSetToCs()
        {
            var folderComponent = CreateProjectWith();
            var fileComponent = (folderComponent as FolderComposite).GetAllFiles().First();

            new JsonReportFileComponent(fileComponent).Language.ShouldBe("cs");
        }

        [Fact]
        public void JsonReportFileComponent_ShouldContainOriginalSource()
        {
            var folderComponent = CreateProjectWith();
            var fileComponent = (folderComponent as FolderComposite).GetAllFiles().First();

            new JsonReportFileComponent(fileComponent).Source.ShouldBe(fileComponent.SourceCode);
        }

        [Fact]
        public void JsonReportFileComponents_ShouldContainMutants()
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
        public void JsonReportFileComponent_DoesNotContainDuplicateMutants()
        {
            var loggerMock = Mock.Of<ILogger>();
            var folderComponent = CreateProjectWith(duplicateMutant: true);
            foreach (var file in (folderComponent as FolderComposite).GetAllFiles())
            {
                var jsonReportComponent = new JsonReportFileComponent(file, loggerMock);
                foreach (var mutant in file.Mutants)
                {
                    jsonReportComponent.Mutants.ShouldContain(m => m.Id == mutant.Id);
                }
            }
        }

        [Fact]
        public void JsonReport_ThresholdsAreSet()
        {
            var folderComponent = CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(), folderComponent);

            report.ShouldSatisfyAllConditions(
                () => report.Thresholds.ShouldContainKey("high"),
                () => report.Thresholds.ShouldContainKey("low"));
        }

        [Fact]
        public void JsonReport_ShouldContainAtLeastOneFile()
        {
            var folderComponent = CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(), folderComponent);

            report.Files.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void JsonReport_WithMutationScoreOverThresholdHighHasGoodHealth()
        {
            var folderComponent = CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 20, thresholdLow: 10), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Good);
        }

        [Fact]
        public void JsonReport_WithMutationScoreEqualToThresholdHighHasGoodHealth()
        {
            var folderComponent = CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 66, thresholdLow: 10), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Good);
        }

        [Fact]
        public void JsonReport_WithMutationScoreBetweenThresholdHighAndBreakHasWarningHealth()
        {
            var folderComponent = CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 80, thresholdBreak: 50), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Warning);
        }

        [Fact]
        public void JsonReport_WithMutationScoreUnderThresholdBreakHasDangerHealth()
        {
            var folderComponent = CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 80, thresholdLow: 70, thresholdBreak: 67), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Danger);
        }

        [Fact]
        public void JsonReport_WithMutationScoreEqualToThresholdBreakHasDangerHealth()
        {
            var folderComponent = CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 80, thresholdLow: 70, thresholdBreak: 66), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Danger);
        }

        [Fact]
        public void JsonReport_BuildReportReturnsSingletonJsonReport()
        {
            var folderComponent = CreateProjectWith();
            var options = new StrykerOptions();

            var firstReport = JsonReport.Build(options, folderComponent);
            var secondReport = JsonReport.Build(options, folderComponent);

            secondReport.ShouldBe(firstReport);
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new JsonReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(CreateProjectWith());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.json");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }

        private IReadOnlyInputComponent CreateProjectWith(bool duplicateMutant = false, int mutationScore = 60)
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

            var folder = new FolderComposite { Name = "RootFolder", RelativePath = "src" };

            for (var i = 1; i <= 2; i++)
            {
                var addedFolder = new FolderComposite { Name = $"{i}", RelativePath = $"src/{i}" };
                folder.Add(addedFolder);

                for (var y = 0; y <= 4; y++)
                {
                    var m = new Collection<Mutant>();
                    addedFolder.Add(new FileLeaf()
                    {
                        Name = $"SomeFile{i}.cs",
                        RelativePath = $"src/{i}/SomeFile{i}.cs",
                        Mutants = m,
                        SourceCode = "void M(){ int i = 0 + 8; }"
                    });

                    for (var z = 0; z <= 5; z++)
                    {
                        m.Add(new Mutant()
                        {
                            Id = duplicateMutant ? 2 : new Random().Next(1, 5000),
                            ResultStatus = 100 / 6 * z < mutationScore ? MutantStatus.Killed : MutantStatus.Survived,
                            Mutation = mutation
                        });
                    }
                }
            }

            return folder;
        }
    }
}
