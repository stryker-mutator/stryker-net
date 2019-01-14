using Newtonsoft.Json;
using Shouldly;
using System.IO;
using System.Linq;
using Xunit;
using static Stryker.Core.Reporters.Json.JsonReporter;

namespace IntegrationTests
{
    public class ValidateStrykerResults
    {
        [Fact]
        public void NetCore2_0()
        {
            var directory = new DirectoryInfo("../../../../../NetCore2_0/ExampleProject.XUnit/StrykerOutput/reports");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles("*.json")
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReportComponent>(strykerRunOutput);

            report.TotalMutants.ShouldBe(15);
            report.SkippedMutants.ShouldBe(1);
            report.SurvivedMutants.ShouldBe(12);
            report.KilledMutants.ShouldBe(2);
            report.TimeoutMutants.ShouldBe(1);
        }

        [Fact]
        public void NetCore2_1()
        {
            var directory = new DirectoryInfo("../../../../../NetCore2_1/ExampleProject.XUnit/StrykerOutput/reports");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles("*.json")
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReportComponent>(strykerRunOutput);

            report.TotalMutants.ShouldBe(15);
            report.SkippedMutants.ShouldBe(1);
            report.SurvivedMutants.ShouldBe(12);
            report.KilledMutants.ShouldBe(2);
            report.TimeoutMutants.ShouldBe(1);
        }
    }
}
