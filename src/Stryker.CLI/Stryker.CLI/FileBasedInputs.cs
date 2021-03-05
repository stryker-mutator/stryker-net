using Newtonsoft.Json;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class FileBasedInputs
    {
        public ProjectInfo ProjectInfo { get; set; }

        public int Concurrency { get; set; }

        public string MutationLevel { get; set; }

        public string[] Mutate { get; set; }
        public string Solution { get; set; }
        public string Project { get; set; }

        public Thresholds Thresholds { get; set; }

        public bool LogToFile { get; set; }
        public string Verbosity { get; set; }
        public string[] Reporters { get; set; }

        // since
        [JsonProperty(PropertyName = "since")]
        public bool Since { get; set; }
        [JsonProperty(PropertyName = "since-branch")]
        public string SinceBranch { get; set; }
        [JsonProperty(PropertyName = "since-commit")]
        public string SinceCommit { get; set; }


        public BaseLine BaseLine { get; set; }
    }

    public class BaseLine
    {

        [JsonProperty(PropertyName = "with-baseline")]
        public bool WithBaseline { get; set; }
        [JsonProperty(PropertyName = "with-baseline")]
        public string Provider { get; set; }
        [JsonProperty(PropertyName = "with-baseline")]
        public string AzureFileShareUrl { get; set; }
        [JsonProperty(PropertyName = "fallback-version")]
        public string FallbackVersion { get; set; }
    }

    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Module { get; set; }
        public string Version { get; set; }
    }
}
