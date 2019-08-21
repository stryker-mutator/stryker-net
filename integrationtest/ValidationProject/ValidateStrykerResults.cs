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

            CheckReportMutantCounts(report, total: 25, skipped: 8, survived: 12, killed: 2, timeout: 1);
        }

        [Fact]
        public void NetStandard()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/NetStandardTestProject.XUnit/StrykerOutput");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutantCounts(report, total: 25, skipped: 0, survived: 20, killed: 2, timeout: 1);
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

                CheckReportMutantCounts(report, total: 3, skipped: 0, survived: 1, killed: 2, timeout: 0);
            }
        }

        private void CheckReportMutantCounts(JsonReport report, int total, int skipped, int survived, int killed, int timeout)
        {
            report.Files.Select(f => f.Value.Mutants.Count()).Sum().ShouldBe(total);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Skipped.ToString())).Sum().ShouldBe(skipped);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Survived.ToString())).Sum().ShouldBe(survived);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Killed.ToString())).Sum().ShouldBe(killed);
            report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Timeout.ToString())).Sum().ShouldBe(timeout);
        }
    }
}
