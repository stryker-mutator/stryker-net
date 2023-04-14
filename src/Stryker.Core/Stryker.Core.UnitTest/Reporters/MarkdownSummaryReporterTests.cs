using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Spectre.Console.Testing;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class MarkdownSummaryReporterTests : TestBase
    {
        [Fact]
        public void MarkdownSummaryReporter_ShouldGenerateReportOnReportDone()
        {
            // Arrange
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 75, Low = 50, Break = 10 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-summary"
            };
            var mockFileSystem = new MockFileSystem();
            var console = new TestConsole().EmitAnsiSequences().Width(160);

            var reportGenerator = new MarkdownSummaryReporter(options, mockFileSystem, console);

            // Act
            reportGenerator.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), null);

            // Assert
            var reportPath = Path.Combine(options.ReportPath, "mutation-summary.md");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }

        [Fact]
        public void MarkdownSummaryReporter_ShouldReportCorrectThresholds()
        {
            // Arrange
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 75, Low = 50, Break = 10 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-summary"
            };
            var mockFileSystem = new MockFileSystem();
            var console = new TestConsole().EmitAnsiSequences().Width(160);

            var reportGenerator = new MarkdownSummaryReporter(options, mockFileSystem, console);

            // Act
            reportGenerator.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), null);

            // Assert
            var reportPath = Path.Combine(options.ReportPath, "mutation-summary.md");
            var fileContents = mockFileSystem.File.ReadAllText(reportPath);

            fileContents.ShouldContain("high:75");
            fileContents.ShouldContain("low:50");
            fileContents.ShouldContain("break:10");
        }

        [Fact]
        public void MarkdownSummaryReporter_ShouldReportCorrectMutationCoverageValues()
        {
            // Arrange
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 75, Low = 50, Break = 10 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-summary"
            };
            var mockFileSystem = new MockFileSystem();
            var console = new TestConsole().EmitAnsiSequences().Width(160);

            var reportGenerator = new MarkdownSummaryReporter(options, mockFileSystem, console);
            var mockReport = ReportTestHelper.CreateProjectWith();

            // Act
            reportGenerator.OnAllMutantsTested(mockReport, null);

            // Assert
            var files = mockReport.GetAllFiles();
            var reportPath = Path.Combine(options.ReportPath, "mutation-summary.md");
            var fileContents = mockFileSystem.File.ReadAllText(reportPath);

            // Spaces are unpredictable - remove them for this comparison.
            var stippedFileContents = fileContents.Replace(" ", string.Empty);

            foreach (var file in files)
            {
                var escapedFilename = file.RelativePath.Replace("/", "\\/");
                stippedFileContents.ShouldContain($"|{escapedFilename}|{file.GetMutationScore() * 100:N2}%|");
            }
        }

        [Fact]
        public void MarkdownSummaryReporter_ShouldOutputSummaryLocationToTheConsole()
        {
            // Arrange
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 75, Low = 50, Break = 10 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-summary"
            };
            var mockFileSystem = new MockFileSystem();
            var console = new TestConsole().EmitAnsiSequences().Width(160);

            var reportGenerator = new MarkdownSummaryReporter(options, mockFileSystem, console);
            var mockReport = ReportTestHelper.CreateProjectWith();

            // Act
            reportGenerator.OnAllMutantsTested(mockReport, null);

            // Assert
            var expectedSummaryReportPath = $"{Path.Join(options.ReportPath, options.ReportFileName)}.md".Replace("\\", "/");
            console.Output.ShouldContain(expectedSummaryReportPath);
            console.Output.GreenSpanCount().ShouldBe(2);
        }

        [Fact]
        public void MarkdownSummaryReporter_ShouldNotOutputForEmptyProject()
        {
            // Arrange
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 75, Low = 50, Break = 10 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-summary"
            };
            var mockFileSystem = new MockFileSystem();
            var console = new TestConsole().EmitAnsiSequences().Width(160);

            var reportGenerator = new MarkdownSummaryReporter(options, mockFileSystem, console);
            var emptyReport = new CsharpFolderComposite() { FullPath = "/home/user/src/project/", RelativePath = "" };

            // Act
            reportGenerator.OnAllMutantsTested(emptyReport, null);

            // Assert
            mockFileSystem.AllFiles.ShouldBeEmpty();
        }
    }
}
