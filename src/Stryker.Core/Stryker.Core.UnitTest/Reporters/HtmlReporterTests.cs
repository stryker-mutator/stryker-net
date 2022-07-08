using Moq;
using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters.Html.reporter;
using Stryker.Core.Reporters.HtmlReporter.ProcessWrapper;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class HtmlReporterTests : TestBase
    {
        [Fact]
        public void ShouldWriteJsonToFile()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, processWrapper: mockProcess.Object);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }

        [Fact]
        public void ShouldReplacePlaceholdersInHtmlFile()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, processWrapper: mockProcess.Object);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldNotContain("##REPORT_JS##");
            fileContents.ShouldNotContain("##REPORT_TITLE##");
            fileContents.ShouldNotContain("##REPORT_JSON##");
        }

        [Fact]
        public void ShouldSupportSpacesInPath()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = " folder \\ next level",
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, processWrapper: mockProcess.Object);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldNotContain("##REPORT_JS##");
            fileContents.ShouldNotContain("##REPORT_TITLE##");
            fileContents.ShouldNotContain("##REPORT_JSON##");
        }

        [Fact]
        public void ShouldContainJsonInHtmlReportFile()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, processWrapper: mockProcess.Object);
            var mutationTree = JsonReportTestHelper.CreateProjectWith();

            reporter.OnAllMutantsTested(mutationTree);
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldContain(@"""thresholds"":{");
            fileContents.ShouldContain(@"""high"":80");
            fileContents.ShouldContain(@"""low"":60");
        }

        [Fact]
        public void ShouldOpenHtmlReportIfOptionIsProvided()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Html,
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };

            var reporter = new HtmlReporter(options, mockFileSystem, processWrapper: mockProcess.Object);
            var mutationTree = JsonReportTestHelper.CreateProjectWith();

            reporter.OnAllMutantsTested(mutationTree);

            var reportUri = Path.Combine(options.ReportPath, $"{options.ReportFileName}.html");
            reportUri = "file://" + reportUri.Replace("\\", "/");

            // Check if browser open action is invoked
            mockProcess.Verify(m => m.Open(reportUri));
        }

        [Theory]
        [InlineData(ReportType.Dashboard)]
        [InlineData(null)]
        public void ShouldNotOpenHtmlReportIfOptionIsProvided(ReportType? reportType)
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                ReportTypeToOpen = reportType,
                OutputPath = Directory.GetCurrentDirectory()
            };

            var reporter = new HtmlReporter(options, mockFileSystem, processWrapper: mockProcess.Object);
            var mutationTree = JsonReportTestHelper.CreateProjectWith();

            reporter.OnAllMutantsTested(mutationTree);

            // Check if browser open action is invoked
            mockProcess.VerifyNoOtherCalls();
        }
    }
}
