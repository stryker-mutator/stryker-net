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
            ApplicationLogging.ConfigureLogger(new LogOptions(Serilog.Events.LogEventLevel.Fatal, false, null));
            ApplicationLogging.LoggerFactory.CreateLogger<HtmlReporterTests>();
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldReplacePlaceholdersInHtmlFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldSatisfyAllConditions(
                () => fileContents.ShouldNotContain("##REPORT_JS##"),
                () => fileContents.ShouldNotContain("##REPORT_TITLE##"),
                () => fileContents.ShouldNotContain("##REPORT_JSON##"));
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldContainJsonReport()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            JsonReport.Build(options, null).ToJson().ShouldBeSubsetOf(fileContents);
        }
    }
}
