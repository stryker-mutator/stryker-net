using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Html;
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
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }

        [Fact]
        public void ShouldReplacePlaceholdersInHtmlFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldNotContain("##REPORT_JS##");
            fileContents.ShouldNotContain("##REPORT_TITLE##");
            fileContents.ShouldNotContain("##REPORT_JSON##");
        }

        [Fact]
        public void ShouldContainJsonInHtmlReportFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem);
            var mutationTree = JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent();

            reporter.OnAllMutantsTested(mutationTree);
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldContain(@"""thresholds"": {");
            fileContents.ShouldContain(@"""high"": 80");
            fileContents.ShouldContain(@"""low"": 60");
        }
    }
}
