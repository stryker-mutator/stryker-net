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

        public string MutationLevel { get; set; }

        public string[] Mutate { get; set; }
        public string Solution { get; set; }

        public Thresholds Thresholds { get; set; }

        public bool? LogToFile { get; set; }
        public string Verbosity { get; set; }
        public string[] Reporters { get; set; }

        // since
        public bool? Since { get; set; }

        [JsonProperty(PropertyName = "since-target")]
        public string SinceTarget { get; set; }

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
