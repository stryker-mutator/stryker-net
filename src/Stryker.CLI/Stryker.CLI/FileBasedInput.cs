using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Stryker.CLI
{
    public interface IExtraData
    {
        Dictionary<string, JsonElement> ExtraData { get; init; }
    }

    public class FileBasedInputOuter
    {
        [JsonPropertyName("stryker-config")]
        [YamlMember(Alias ="stryker-config")]
        public FileBasedInput Input { get; init; }
    }

    public class FileBasedInput : IExtraData
    {
        [JsonPropertyName("project-info")]
        public ProjectInfo ProjectInfo { get; init; }

        [JsonPropertyName("concurrency")]
        public int? Concurrency { get; init; }

        [JsonPropertyName("mutation-level")]
        public string MutationLevel { get; init; }

        [JsonPropertyName("language-version")]
        public string LanguageVersion { get; init; }

        [JsonPropertyName("additional-timeout")]
        public int? AdditionalTimeout { get; init; }

        [JsonPropertyName("mutate")]
        public string[] Mutate { get; init; }

        [JsonPropertyName("solution")]
        public string Solution { get; init; }

        [JsonPropertyName("target-framework")]
        public string TargetFramework { get; init; }

        [JsonPropertyName("project")]
        public string Project { get; init; }

        [JsonPropertyName("coverage-analysis")]
        public string CoverageAnalysis { get; init; }

        [JsonPropertyName("disable-bail")]
        public bool? DisableBail { get; init; }

        [JsonPropertyName("disable-mix-mutants")]
        public bool? DisableMixMutants { get; init; }

        [JsonPropertyName("thresholds")]
        public ThresholdsConfig Thresholds { get; init; }

        [JsonPropertyName("verbosity")]
        public string Verbosity { get; init; }

        [JsonPropertyName("reporters")]
        public string[] Reporters { get; init; }

        [JsonPropertyName("since")]
        public Since Since { get; init; }

        [JsonPropertyName("baseline")]
        public Baseline Baseline { get; init; }

        [JsonPropertyName("dashboard-url")]
        public string DashboardUrl { get; init; }

        [JsonPropertyName("test-projects")]
        public string[] TestProjects { get; init; }

        [JsonPropertyName("test-case-filter")]
        public string TestCaseFilter { get; init; }

        [JsonPropertyName("ignore-mutations")]
        public string[] IgnoreMutations { get; init; }

        [JsonPropertyName("ignore-methods")]
        public string[] IgnoreMethods { get; init; }

        [JsonPropertyName("report-file-name")]
        public string ReportFileName { get; init; }

        [JsonPropertyName("break-on-initial-test-failure")]
        public bool? BreakOnInitialTestFailure { get; init; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtraData { get; init; }
    }

    public class Since : IExtraData
    {
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; init; }

        [JsonPropertyName("ignore-changes-in")]
        public string[] IgnoreChangesIn { get; init; }

        [JsonPropertyName("target")]
        public string Target { get; init; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtraData { get; init; }
    }

    public class Baseline : IExtraData
    {
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; init; }

        [JsonPropertyName("provider")]
        public string Provider { get; init; }

        [JsonPropertyName("azure-fileshare-url")]
        public string AzureFileShareUrl { get; init; }

        [JsonPropertyName("fallback-version")]
        public string FallbackVersion { get; init; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtraData { get; init; }
    }

    public class ProjectInfo : IExtraData
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("module")]
        public string Module { get; init; }

        [JsonPropertyName("version")]
        public string Version { get; init; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtraData { get; init; }
    }

    public class ThresholdsConfig : IExtraData
    {
        [JsonPropertyName("high")]
        public int? High { get; init; }

        [JsonPropertyName("low")]
        public int? Low { get; init; }

        [JsonPropertyName("break")]
        public int? Break { get; init; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtraData { get; init; }
    }
}
