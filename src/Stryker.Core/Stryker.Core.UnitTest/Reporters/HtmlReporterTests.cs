using Microsoft.Extensions.Logging;
using Shouldly;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class HtmlReporterTests
    {
        public HtmlReporterTests()
        {
            var options = new LogOptions { LogLevel = Serilog.Events.LogEventLevel.Fatal, LogToFile = false };
            ApplicationLogging.ConfigureLogger(options, null);
            ApplicationLogging.LoggerFactory.CreateLogger<HtmlReporterTests>();
        }

        [Fact]
        public void ShouldWriteJsonToFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory()
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
                OutputPath = Directory.GetCurrentDirectory()
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
                OutputPath = Directory.GetCurrentDirectory()
            };
            var reporter = new HtmlReporter(options, mockFileSystem);
            var mutationTree = JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent();

            reporter.OnAllMutantsTested(mutationTree);
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            JsonReport.ReportCache.ToJson().ShouldBeSubsetOf(fileContents);
        }
    }
}
