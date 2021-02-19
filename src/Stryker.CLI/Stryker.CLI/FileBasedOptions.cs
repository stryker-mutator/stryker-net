using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class FileBasedOptions
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

        public Incremental Incremental { get; set; }
    }

    public class Incremental
    {
        public string Target { get; set; }
        public bool Since { get; set; }
        public bool WithBaseline { get; set; }
        public string Provider { get; set; }
        public string AzureFileShareUrl { get; set; }
    }

    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Module { get; set; }
        public string Version { get; set; }
    }
}
