using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Abstractions.Options;

/// <summary>
/// Isolate build related options
/// </summary>
/// <remarks>Intent is to improve clarity for options</remarks>
public interface IStrykerBuildOptions
{
    string? MsBuildPath { get; init; }
    string Configuration { get; init; }
    string Platform { get; }
    string TargetFramework { get; init; }
    LanguageVersion LanguageVersion { get; init; }
    string WorkingDirectory { get; init; }
    string? SolutionPath { get; init; }
}

public interface IStrykerOptions : IStrykerBuildOptions
{
    int AdditionalTimeout { get; init; }
    string AzureFileStorageSas { get; init; }
    string AzureFileStorageUrl { get; init; }
    string S3BucketName { get; init; }
    string S3Endpoint { get; init; }
    string S3Region { get; init; }
    BaselineProvider BaselineProvider { get; init; }
    string BaselineOutputPath { get; init; }
    bool BreakOnInitialTestFailure { get; set; }
    int Concurrency { get; init; }
    string DashboardApiKey { get; init; }
    string DashboardUrl { get; init; }
    bool DiagMode { get; init; }
    IEnumerable<IExclusionPattern> DiffIgnoreChanges { get; init; }
    IEnumerable<LinqExpression> ExcludedLinqExpressions { get; init; }
    IEnumerable<Mutator> ExcludedMutations { get; init; }
    string FallbackVersion { get; init; }
    IEnumerable<Regex> IgnoredMethods { get; init; }
    bool IsSolutionContext { get; }
    ILogOptions LogOptions { get; init; }
    string ModuleName { get; init; }
    IEnumerable<IFilePattern> Mutate { get; init; }
    MutationLevel MutationLevel { get; init; }
    OptimizationModes OptimizationMode { get; init; }
    string OutputPath { get; init; }
    string ProjectName { get; set; }
    string ProjectPath { get; init; }
    string ProjectVersion { get; set; }
    IEnumerable<Reporter> Reporters { get; init; }
    string ReportFileName { get; init; }
    string ReportPath { get; }
    ReportType? ReportTypeToOpen { get; init; }
    bool Since { get; init; }
    string SinceTarget { get; init; }
    string SourceProjectName { get; init; }
    string TestCaseFilter { get; init; }
    IEnumerable<string> TestProjects { get; init; }
    TestRunner TestRunner { get; init; }
    IThresholds Thresholds { get; init; }
    bool WithBaseline { get; init; }
    IProvideId MutantIdProvider { get; set; }
}
