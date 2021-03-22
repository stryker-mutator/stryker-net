using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options
{
    public interface IStrykerOptions
    {
        int AdditionalTimeoutMS { get; }
        string BasePath { get; }
        int ConcurrentTestrunners { get; }
        bool DevMode { get; }
        IEnumerable<Mutator> ExcludedMutations { get; }
        IEnumerable<FilePattern> FilePatterns { get; }
        IEnumerable<Regex> IgnoredMethods { get; }
        LanguageVersion LanguageVersion { get; }
        LogOptions LogOptions { get; }
        string OptimizationMode { get; set; }
        OptimizationFlags Optimizations { get; }
        string OutputPath { get; }
        string ProjectUnderTestNameFilter { get; }
        IEnumerable<Reporter> Reporters { get; }
        string SolutionPath { get; }
        IEnumerable<string> TestProjects { get; set; }
        TestRunner TestRunner { get; set; }
        Threshold Thresholds { get; }
        string AzureSAS { get; }
        string AzureFileStorageUrl { get; set; }
        BaselineProvider BaselineProvider { get; }
        MutationLevel MutationLevel { get; }
        string ProjectUnderTest { get; }
        bool DiffEnabled { get; }
        string GitDiffSource { get; }
        string ModuleName { get; }
        string ProjectName { get; }
        string ProjectVersion { get; }
        bool CompareToDashboard { get; }
        string FallbackVersion { get; }
        IEnumerable<FilePattern> DiffIgnoreFiles { get; }
        string DashboardUrl { get; }
        string DashboardApiKey { get; }
    }
}
