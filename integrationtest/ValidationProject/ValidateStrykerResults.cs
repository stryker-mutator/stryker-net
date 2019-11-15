using Newtonsoft.Json;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Json;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace IntegrationTests
{
    public class ValidateStrykerResults
    {
        private const string MutationReportJson = "mutation-report.json";

        [Fact]
        public void NetCore()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/NetCoreTestProject.XUnit/StrykerOutput");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutantCounts(report, total: 63, skipped: 21, survived: 2, killed: 2, timeout: 2, nocoverage: 34);
        }

        [Fact]
        public void NetFullFramework()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var directory = new DirectoryInfo("../../../../TargetProjects/NetFramework/FullFrameworkApp.Test/StrykerOutput");
                directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

                var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();

                var strykerRunOutput = File.ReadAllText(latestReport.FullName);

                var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

                CheckReportMutantCounts(report, total: 12, skipped: 0, survived: 1, killed: 4, timeout: 0, nocoverage: 7);
            }
        }

        private void CheckReportMutantCounts(JsonReport report, int total, int skipped, int survived, int killed, int timeout, int nocoverage)
        {
            var actualTotal = report.Files.Select(f => f.Value.Mutants.Count()).Sum();
            var actualSkipped = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Skipped.ToString())).Sum();
            var actualSurvived = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Survived.ToString())).Sum();
            var actualKilled = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Killed.ToString())).Sum();
            var actualTimeout = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Timeout.ToString())).Sum();
            var actualNoCoverage = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.NoCoverage.ToString())).Sum();

            report.Files.ShouldSatisfyAllConditions(
                () => actualTotal.ShouldBe(total),
                () => actualSkipped.ShouldBe(skipped),
                () => actualSurvived.ShouldBe(survived),
                () => actualKilled.ShouldBe(killed),
                () => actualTimeout.ShouldBe(timeout),
                () => actualNoCoverage.ShouldBe(nocoverage)
            );
        }
    }
}
