using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class JsonReporterTests : TestBase
    {
        [Fact]
        public void JsonMutantPositionLine_ThrowsArgumentExceptionWhenSetToLessThan1()
        {
            Should.Throw<ArgumentException>(() => new Position().Line = -1);
            Should.Throw<ArgumentException>(() => new Position().Line = 0);
        }

        [Fact]
        public void JsonMutantPositionColumn_ThrowsArgumentExceptionWhenSetToLessThan1()
        {
            Should.Throw<ArgumentException>(() => new Position().Column = -1);
            Should.Throw<ArgumentException>(() => new Position().Column = 0);
        }

        [Fact]
        public void JsonMutantLocation_FromValidFileLinePositionSpanShouldAdd1ToLineAndColumnNumbers()
        {
            var lineSpan = new FileLinePositionSpan(
                "",
                new LinePosition(2, 2),
                new LinePosition(4, 5));

            var jsonMutantLocation = new Core.Reporters.Json.Location(lineSpan);

            jsonMutantLocation.Start.Line.ShouldBe(3);
            jsonMutantLocation.Start.Column.ShouldBe(3);
            jsonMutantLocation.End.Line.ShouldBe(5);
            jsonMutantLocation.End.Column.ShouldBe(6);
        }

        [Fact]
        public void JsonReportFileComponent_ShouldHaveLanguageSetToCs()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
            var fileComponent = ((CsharpFileLeaf)(folderComponent as CsharpFolderComposite).GetAllFiles().First()).ToReadOnly();

            new SourceFile(fileComponent).Language.ShouldBe("cs");
        }

        [Fact]
        public void JsonReportFileComponent_ShouldContainOriginalSource()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
            var fileComponent = ((CsharpFileLeaf)(folderComponent as CsharpFolderComposite).GetAllFiles().First()).ToReadOnly();

            new SourceFile(fileComponent).Source.ShouldBe(fileComponent.SourceCode);
        }

        [Fact]
        public void JsonReportFileComponents_ShouldContainMutants()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
            foreach (var file in (folderComponent as CsharpFolderComposite).GetAllFiles())
            {
                var jsonReportComponent = new SourceFile(((CsharpFileLeaf)file).ToReadOnly());
                foreach (var mutant in file.Mutants)
                {
                    jsonReportComponent.Mutants.ShouldContain(m => m.Id == mutant.Id.ToString());
                }
            }
        }

        [Fact]
        public void JsonReportFileComponent_DoesNotContainDuplicateMutants()
        {
            var loggerMock = Mock.Of<ILogger>();
            var folderComponent = JsonReportTestHelper.CreateProjectWith(duplicateMutant: true);
            foreach (var file in (folderComponent as CsharpFolderComposite).GetAllFiles())
            {
                var jsonReportComponent = new SourceFile(((CsharpFileLeaf)file).ToReadOnly(), loggerMock);
                foreach (var mutant in file.Mutants)
                {
                    jsonReportComponent.Mutants.ShouldContain(m => m.Id == mutant.Id.ToString());
                }
            }
        }

        [Fact]
        public void JsonReport_ThresholdsAreSet()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(), folderComponent.ToReadOnlyInputComponent(), It.IsAny<TestProjectsInfo>());

            report.ShouldSatisfyAllConditions(
                () => report.Thresholds.ShouldContainKey("high"),
                () => report.Thresholds.ShouldContainKey("low"));
        }

        [Fact]
        public void JsonReport_ShouldContainAtLeastOneFile()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(), folderComponent.ToReadOnlyInputComponent(), It.IsAny<TestProjectsInfo>());

            report.Files.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void JsonReport_ShouldContainTheProjectRoot()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(), folderComponent.ToReadOnlyInputComponent(), It.IsAny<TestProjectsInfo>());

            report.ProjectRoot.ShouldBe("/home/user/src/project/");
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new JsonReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent(), It.IsAny<TestProjectsInfo>());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.json");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
            var fileContents = mockFileSystem.File.ReadAllText(reportPath);
            fileContents.ShouldContain(@"""thresholds"":{");
            fileContents.ShouldContain(@"""high"":80");
            fileContents.ShouldContain(@"""low"":60");
        }
    }
}
