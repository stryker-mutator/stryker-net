using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class TestStatisticsReporterTests
    {
        [Fact]
        public void ShouldContainAtLeastTenMutants()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            
            var report = TestStatisticsReporter.Build(new StrykerOptions(), folderComponent, 
                folderComponent.DetectedMutants.SelectMany(x => x.CoveringTest.Keys).Select(t => new TestDescription(t, $"test {t}")));

            report.Mutants.Count.ShouldBeGreaterThan(10);
        }

        [Fact]
        public void ShouldContainAtLeastTenTests()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
            var allTests = folderComponent.DetectedMutants.SelectMany(x => x.CoveringTest.Keys).Select(t => new TestDescription(t, $"test {t}"));
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
            reporter.OnStartMutantTestRun(readOnlyInputComponent.DetectedMutants, readOnlyInputComponent.DetectedMutants.SelectMany( m => m.CoveringTest.Keys).Select(t => new TestDescription(t, $"test {t}")));
            reporter.OnAllMutantsTested(readOnlyInputComponent);
            var reportPath = Path.Combine(options.OutputPath, "reports", "test-stats-report.json");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }
    }
}
