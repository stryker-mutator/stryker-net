using Newtonsoft.Json;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class FileBasedInputOuter
    {
        [JsonProperty(PropertyName = "stryker-config")]
        public FileBasedInput Input { get; set; }
    }

    public class FileBasedInput
    {
        [JsonProperty(PropertyName = "project-info")]
        public ProjectInfo ProjectInfo { get; set; }

        public int? Concurrency { get; set; }

        [JsonProperty(PropertyName = "mutation-level")]
        public string MutationLevel { get; set; }

        [JsonProperty(PropertyName = "language-version")]
        public string LanguageVersion { get; set; }

        [JsonProperty(PropertyName = "additional-timeout")]
        public int AdditionalTimeout { get; set; }

        public string[] Mutate { get; set; }

        public string Solution { get; set; }

        public string Project { get; set; }

        [JsonProperty(PropertyName = "coverage-analysis")]
        public string CoverageAnalysis { get; set; }

        [JsonProperty(PropertyName = "disable-bail")]
        public bool DisableBail { get; set; }

        [JsonProperty(PropertyName = "disable-testing-mix-mutations")]
        public bool DisableTestingMixMutations { get; set; }

        public Thresholds Thresholds { get; set; }

        public string Verbosity { get; set; }

        public string[] Reporters { get; set; }

        // since
        public bool? Since { get; set; }

        [JsonProperty(PropertyName = "since-target")]
        public string SinceTarget { get; set; }
        
        [JsonProperty(PropertyName = "with-baseline")]
        public bool WithBaseline { get; set; }

        [JsonProperty(PropertyName = "dashboard-url")]
        public string DashboardUrl { get; set; }

        [JsonProperty(PropertyName = "test-projects")]
        public string[] TestProjects { get; set; }
        
        [JsonProperty(PropertyName = "ignore-mutations")]
        public string[] IgnoreMutations { get; set; }
        
        [JsonProperty(PropertyName = "ignore-methods")]
        public string[] IgnoreMethods { get; set; }

        public BaseLine BaseLine { get; set; }
    }

    public class BaseLine
    {
        [JsonProperty(PropertyName = "with-baseline")]
        public bool? WithBaseline { get; set; }

        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }

        [JsonProperty(PropertyName = "azure-fileshare-url")]
        public string AzureFileShareUrl { get; set; }

        [JsonProperty(PropertyName = "fallback-version")]
        public string FallbackVersion { get; set; }

        [JsonProperty(PropertyName = "ignore-changed-files")]
        public string[] IgnoreChangedFiles { get; set; }
    }

    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Module { get; set; }
        public string Version { get; set; }
    }
}
