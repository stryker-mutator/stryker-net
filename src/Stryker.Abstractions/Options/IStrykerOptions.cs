using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;

namespace Stryker.Abstractions.Options;

public interface IStrykerOptions
{
    int AdditionalTimeout { get; init; }
    string AzureFileStorageSas { get; init; }
    string AzureFileStorageUrl { get; init; }
    BaselineProvider BaselineProvider { get; init; }
    bool BreakOnInitialTestFailure { get; set; }
    int Concurrency { get; init; }
    string Configuration { get; init; }
    string DashboardApiKey { get; init; }
    string DashboardUrl { get; init; }
    bool DevMode { get; init; }
    IEnumerable<IExclusionPattern> DiffIgnoreChanges { get; init; }
    IEnumerable<LinqExpression> ExcludedLinqExpressions { get; init; }
    IEnumerable<Mutator> ExcludedMutations { get; init; }
    string FallbackVersion { get; init; }
    IEnumerable<Regex> IgnoredMethods { get; init; }
    bool IsSolutionContext { get; }
    LanguageVersion LanguageVersion { get; init; }
    ILogOptions LogOptions { get; init; }
    string ModuleName { get; init; }
    string MsBuildPath { get; init; }
    IEnumerable<IFilePattern> Mutate { get; init; }
    MutationLevel MutationLevel { get; init; }
    OptimizationModes OptimizationMode { get; init; }
    string OutputPath { get; init; }
    string ProjectName { get; set; }
    string ProjectPath { get; init; }
    string ProjectVersion { get; set; }
    IEnumerable<IReporter> Reporters { get; init; }
    string ReportFileName { get; init; }
    string ReportPath { get; }
    ReportType? ReportTypeToOpen { get; init; }
    bool Since { get; init; }
    string SinceTarget { get; init; }
    string SolutionPath { get; init; }
    string SourceProjectName { get; init; }
    string TargetFramework { get; init; }
    string TestCaseFilter { get; init; }
    IEnumerable<string> TestProjects { get; init; }
    IThresholds Thresholds { get; init; }
    bool WithBaseline { get; init; }
    string WorkingDirectory { get; init; }
}
