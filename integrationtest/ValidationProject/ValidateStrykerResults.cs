using Newtonsoft.Json;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace IntegrationTests
{
    public class ValidateStrykerResults
    {
        [Fact]
        public void NetCore2_0()
        {
            var directory = new DirectoryInfo("../TargetProjects/TestProject.XUnit/StrykerOutput/reports");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles("*.json")
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutantCounts(report, total: 15, skipped: 1, survived: 12, killed: 2, timeout: 1);
        }

        [Fact]
        public void NetCore2_1()
        {
            var directory = new DirectoryInfo("../TargetProjects/TestProject.XUnit/StrykerOutput/reports");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles("*.json")
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutantCounts(report, total: 15, skipped: 1, survived: 12, killed: 2, timeout: 1);
        }

        private void CheckReportMutantCounts(JsonReport report, int total, int skipped, int survived, int killed, int timeout)
        {
            report.Files.Select(f => f.Value.Mutants.Count()).Sum().ShouldBe(total);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Skipped.ToString())).Sum().ShouldBe(skipped);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Survived.ToString())).Sum().ShouldBe(survived);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Killed.ToString())).Sum().ShouldBe(killed);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Timeout.ToString())).Sum().ShouldBe(timeout);
        }

        private void CheckMutantSpans(JsonReport report)
        {



        }

        private IEnumerable<JsonMutant> GenerateJsonMutants()
        {
            var mutants = new List<JsonMutant>
            {
                new JsonMutant
                {
                    Id = 0,
                    Location = new JsonMutantLocation(
                        new JsonMutantPosition
                        {
                            Column = 0,
                            Line = 0
                        },
                        new JsonMutantPosition
                        {
                            Column = 0,
                            Line = 0
                        })
                }
            };

            return mutants;
        }
    }
}
