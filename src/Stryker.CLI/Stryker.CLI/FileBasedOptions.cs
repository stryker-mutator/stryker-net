using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class FileBasedOptions
    {
        public ProjectInfo ProjectInfo { get; }

        public string Concurrency { get; }

        public string MutationLevel { get; }

        public string Mutate { get; }
        public string Solution { get; }
        public string Project { get; }

        public Thresholds Thresholds { get; }

        public string LogToFile { get; }
        public string Verbosity { get; }
        public string[] Reporters { get; }

        public Incremental Incremental { get; }
    }

    public class Incremental
    {
        public string Target { get; }
        public bool Since { get; }
        public bool WithBaseline { get; }

        public string Provider { get; }
        public string AzureFileShareUrl { get; }
    }

    public class ProjectInfo
    {
        public string Name { get; }
        public string Module { get; }
        public string Version { get; }
    }
}
