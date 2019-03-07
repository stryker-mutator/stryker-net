using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
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
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
            var fileComponent = (folderComponent as FolderComposite).GetAllFiles().First();

            new JsonReportFileComponent(fileComponent).Language.ShouldBe("cs");
        }

        [Fact]
        public void JsonReportFileComponent_ShouldContainOriginalSource()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
            var fileComponent = (folderComponent as FolderComposite).GetAllFiles().First();

            new JsonReportFileComponent(fileComponent).Source.ShouldBe(fileComponent.SourceCode);
        }

        [Fact]
        public void JsonReportFileComponents_ShouldContainMutants()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
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
            var folderComponent = JsonReportTestHelper.CreateProjectWith(duplicateMutant: true);
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
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(), folderComponent);

            report.ShouldSatisfyAllConditions(
                () => report.Thresholds.ShouldContainKey("high"),
                () => report.Thresholds.ShouldContainKey("low"));
        }

        [Fact]
        public void JsonReport_ShouldContainAtLeastOneFile()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(), folderComponent);

            report.Files.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void JsonReport_WithMutationScoreOverThresholdHighHasGoodHealth()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 20, thresholdLow: 10), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Good);
        }

        [Fact]
        public void JsonReport_WithMutationScoreEqualToThresholdHighHasWarningHealth()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 67, thresholdLow: 10), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Warning);
        }

        [Fact]
        public void JsonReport_WithMutationScoreBetweenThresholdHighInclusiveAndLowNonInclusiveHasWarningHealth()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 67, thresholdLow: 66), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Warning);
        }

        [Fact]
        public void JsonReport_WithMutationScoreEqualToThresholdLowHasWarningHealth()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 80, thresholdLow: 65), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Warning);
        }

        [Fact]
        public void JsonReport_WithMutationScoreUnderThresholdLowHasDangerHealth()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();

            var report = JsonReport.Build(new StrykerOptions(thresholdHigh: 80, thresholdLow: 67), folderComponent);

            report.Files.First().Value.Health.ShouldBe(Health.Danger);
        }

        [Fact]
        public void JsonReport_BuildReportReturnsSingletonJsonReport()
        {
            var folderComponent = JsonReportTestHelper.CreateProjectWith();
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

            reporter.OnAllMutantsTested(JsonReportTestHelper.CreateProjectWith());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.json");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }
    }
}
