using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.TestStatisticsReporter;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class TestStatisticsReporterTests
    {
        [Fact]
        public void ShouldContainAtLeastTenMutants()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = TestStatisticsReporter.Build(new StrykerOptions(), folderComponent, folderComponent.DetectedMutants.SelectMany(x => x.CoveringTest.Keys));

            report.Mutants.Count.ShouldBeGreaterThan(10);
        }

        [Fact]
        public void ShouldContainAtLeastTenTests()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
            var allTests = folderComponent.DetectedMutants.SelectMany(x => x.CoveringTest.Keys);
            var report = TestStatisticsReporter.Build(new StrykerOptions(), folderComponent, allTests);
            report.Tests.Count.ShouldBeGreaterThan(10);
        }
        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new TestStatisticsReporter(options, mockFileSystem);
            var readOnlyInputComponent = JsonReportTestHelper.CreateProjectWith();
            reporter.OnStartMutantTestRun(readOnlyInputComponent.DetectedMutants, readOnlyInputComponent.DetectedMutants.SelectMany( m => m.CoveringTest.Keys));
            reporter.OnAllMutantsTested(readOnlyInputComponent);
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.json");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }
    }
}
