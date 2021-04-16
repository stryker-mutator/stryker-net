using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        public bool DevMode { get; init; }

        public string BasePath { get; init; }
        public string OutputPath { get; init; }
        public string SolutionPath { get; init; }

        public LogOptions LogOptions { get; init; }
        public MutationLevel MutationLevel { get; init; }
        public Thresholds Thresholds { get; init; } = new Thresholds();

        public int AdditionalTimeoutMS { get; init; }
        public LanguageVersion LanguageVersion { get; init; }

        public int Concurrency { get; init; }
        public string ProjectUnderTestName { get; init; }
        public IEnumerable<string> TestProjects { get; init; }

        public bool WithBaseline { get; init; }
        public IEnumerable<Reporter> Reporters { get; init; }

        public BaselineProvider BaselineProvider { get; init; }
        public string AzureFileStorageUrl { get; init; }
        public string AzureFileStorageSas { get; init; }

        public string DashboardUrl { get; init; }
        public string DashboardApiKey { get; init; }
        public string ProjectName { get; init; }

        public bool Since { get; init; }
        public string SinceTarget { get; init; }
        public IEnumerable<FilePattern> DiffIgnoreFilePatterns { get; init; }

        public string FallbackVersion { get; init; }
        public string ProjectVersion { get; init; }
        public string ModuleName { get; init; }

        public IEnumerable<FilePattern> Mutate { get; init; }
        public IEnumerable<Regex> IgnoredMethods { get; init; }
        public IEnumerable<Mutator> ExcludedMutators { get; init; }

        public OptimizationModes OptimizationMode { get; init; }

        public StrykerOptions Copy(string basePath, string projectUnderTest, IEnumerable<string> testProjects)
        {
            return new StrykerOptions()
            {
                AdditionalTimeoutMS = AdditionalTimeoutMS,
                AzureFileStorageSas = AzureFileStorageSas,
                AzureFileStorageUrl = AzureFileStorageUrl,
                BaselineProvider = BaselineProvider,
                BasePath = basePath,
                Concurrency = Concurrency,
                DashboardApiKey = DashboardApiKey,
                DashboardUrl = DashboardUrl,
                DevMode = DevMode,
                Since = Since,
                DiffIgnoreFilePatterns = DiffIgnoreFilePatterns,
                ExcludedMutators = ExcludedMutators,
                FallbackVersion = FallbackVersion,
                IgnoredMethods = IgnoredMethods,
                LanguageVersion = LanguageVersion,
                LogOptions = LogOptions,
                ModuleName = ModuleName,
                Mutate = Mutate,
                MutationLevel = MutationLevel,
                OptimizationMode = OptimizationMode,
                OutputPath = OutputPath,
                ProjectName = ProjectName,
                ProjectUnderTestName = projectUnderTest,
                ProjectVersion = ProjectVersion,
                Reporters = Reporters,
                SinceTarget = SinceTarget,
                SolutionPath = SolutionPath,
                TestProjects = testProjects,
                Thresholds = Thresholds,
                WithBaseline = WithBaseline
            };
        }
    }
}
