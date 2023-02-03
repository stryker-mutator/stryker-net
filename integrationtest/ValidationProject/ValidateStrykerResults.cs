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
        [Trait("Category", "SingleTestProject")]
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

                CheckReportMutantCounts(report, total: 25, ignored: 8, survived: 1, killed: 5, timeout: 0, nocoverage: 11);
            }
        }

        [Fact]
        [Trait("Category", "SingleTestProject")]
        public void NetCore()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/NetCoreTestProject.XUnit/StrykerOutput");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutantCounts(report, total: 112, ignored: 53, survived: 4, killed: 6, timeout: 2, nocoverage: 45);
        }

        [Fact]
        [Trait("Category", "MultipleTestProjects")]
        public void NetCoreWithTwoTestProjects()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/Targetproject/StrykerOutput");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutantCounts(report, total: 112, ignored: 6, survived: 10, killed: 14, timeout: 2, nocoverage: 78);
        }

        [Fact]
        [Trait("Category", "Solution")]
        public void SolutionRun()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/StrykerOutput");
            directory.GetFiles("*.json").ShouldNotBeNull("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutantCounts(report, total: 115, ignored: 54, survived: 4, killed: 8, timeout: 2, nocoverage: 45);
        }

        private void CheckReportMutantCounts(JsonReport report, int total, int ignored, int survived, int killed, int timeout, int nocoverage)
        {
            var actualTotal = report.Files.Select(f => f.Value.Mutants.Count()).Sum();
            var actualIgnored = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Ignored.ToString())).Sum();
            var actualSurvived = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Survived.ToString())).Sum();
            var actualKilled = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Killed.ToString())).Sum();
            var actualTimeout = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Timeout.ToString())).Sum();
            var actualNoCoverage = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.NoCoverage.ToString())).Sum();

            report.Files.ShouldSatisfyAllConditions(
                () => actualTotal.ShouldBe(total),
                () => actualIgnored.ShouldBe(ignored),
                () => actualSurvived.ShouldBe(survived),
                () => actualKilled.ShouldBe(killed),
                () => actualTimeout.ShouldBe(timeout),
                () => actualNoCoverage.ShouldBe(nocoverage)
            );
        }
    }
}
