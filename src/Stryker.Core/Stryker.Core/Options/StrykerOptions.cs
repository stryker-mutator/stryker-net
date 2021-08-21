using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Baseline.Providers;
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
        public Thresholds Thresholds { get; init; } = new Thresholds() { Break = 0, Low = 60, High = 80 };

        public int AdditionalTimeout { get; init; }
        public LanguageVersion LanguageVersion { get; init; }

        public int Concurrency { get; init; }
        public string ProjectUnderTestName { get; init; }
        public IEnumerable<string> TestProjects { get; init; } = Enumerable.Empty<string>();
        public string TestCaseFilter { get; init; }

        public bool WithBaseline { get; init; }
        public IEnumerable<Reporter> Reporters { get; init; } = Enumerable.Empty<Reporter>();

        public BaselineProvider BaselineProvider { get; init; }
        public string AzureFileStorageUrl { get; init; }
        public string AzureFileStorageSas { get; init; }

        public string DashboardUrl { get; init; }
        public string DashboardApiKey { get; init; }
        public string ProjectName { get; init; }

        public bool Since { get; init; }
        public string SinceTarget { get; init; }
        public IEnumerable<FilePattern> DiffIgnoreChanges { get; init; } = Enumerable.Empty<FilePattern>();

        public string FallbackVersion { get; init; }
        public string ProjectVersion { get; init; }
        public string ModuleName { get; init; }

        public IEnumerable<FilePattern> Mutate { get; init; } = new[] { FilePattern.Parse("**/*") };
        public IEnumerable<Regex> IgnoredMethods { get; init; } = Enumerable.Empty<Regex>();
        public IEnumerable<Mutator> ExcludedMutations { get; init; } = Enumerable.Empty<Mutator>();
        public IEnumerable<LinqExpression> ExcludedLinqExpressions { get; init; } = Enumerable.Empty<LinqExpression>();

        public OptimizationModes OptimizationMode { get; init; }

        public StrykerOptions Copy(string basePath, string projectUnderTest, IEnumerable<string> testProjects)
        {
            return new StrykerOptions()
            {
                AdditionalTimeout = AdditionalTimeout,
                AzureFileStorageSas = AzureFileStorageSas,
                AzureFileStorageUrl = AzureFileStorageUrl,
                BaselineProvider = BaselineProvider,
                BasePath = basePath,
                Concurrency = Concurrency,
                DashboardApiKey = DashboardApiKey,
                DashboardUrl = DashboardUrl,
                DevMode = DevMode,
                Since = Since,
                DiffIgnoreChanges = DiffIgnoreChanges,
                ExcludedMutations = ExcludedMutations,
                ExcludedLinqExpressions = ExcludedLinqExpressions,
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
