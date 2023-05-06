using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options;

public class StrykerOptions
{
    /// <summary>
    /// A custom settable path to the msBuild exe for .NET Framework projects. When null stryker will search for the path itself.
    /// </summary>
    public string MsBuildPath { get; init; }
    /// <summary>
    /// If true, stryker will fail when mutants are not being rollbacked.
    /// </summary>
    public bool DevMode { get; init; }

    /// <summary>
    /// The path of the project currently being tested. In the context of solution runs this is custom for each project
    /// </summary>
    public string ProjectPath { get; init; }

    /// <summary>
    /// When true, stryker is mutating all projects in a solution
    /// </summary>
    /// <returns></returns>
    public bool IsSolutionContext => SolutionPath != null && FilePathUtils.NormalizePathSeparators(WorkingDirectory) == FilePathUtils.NormalizePathSeparators(Path.GetDirectoryName(SolutionPath));

    /// <summary>
    /// The path of the root of the scope of stryker.
    /// In the context of a solution run this will be the root of the solution.
    /// In the context of a project run this will be the root of the project under test
    /// In the context of a multi test project run this will be the root of the project under test
    /// </summary>
    public string WorkingDirectory { get => _workingDirectoryField ?? ProjectPath; init => _workingDirectoryField = value; }
    private readonly string _workingDirectoryField;
    /// <summary>
    /// The path all output is written to. For example reports and logging files.
    /// </summary>
    public string OutputPath { get; init; }
    public string ReportPath => Path.Combine(OutputPath ?? ".", "reports");
    /// <summary>
    /// A custom settable name for report files.
    /// </summary>
    public string ReportFileName { get; init; }
    /// <summary>
    /// The full path of the solution file. Can be null.
    /// </summary>
    public string SolutionPath { get; init; }
    /// <summary>
    /// The detected target framework for the current project under test.
    /// </summary>
    public string TargetFramework { get; init; }

    /// <summary>
    /// The options passed to all logging systems
    /// </summary>
    public LogOptions LogOptions { get; init; }

    /// <summary>
    /// Mutators should be disabled when their level is higher than this mutation level.
    /// </summary>
    public MutationLevel MutationLevel { get; init; }

    /// <summary>
    /// Used to set colors in the reports and fail stryker if the mutation score is lower than the break value.
    /// </summary>
    public Thresholds Thresholds { get; init; } = new Thresholds() { Break = 0, Low = 60, High = 80 };

    /// <summary>
    /// The ammount of milliseconds that should be added to the timeout period when testing mutants.
    /// </summary>
    public int AdditionalTimeout { get; init; }

    /// <summary>
    /// The C# language version
    /// </summary>
    public LanguageVersion LanguageVersion { get; init; }

    /// <summary>
    /// This value is used to override max concurrency usage. When set to 4, stryker should spin up 4 test runners in parallel.
    /// </summary>
    public int Concurrency { get; init; }

    /// <summary>
    /// When multiple possible projects are found by stryker, this filter is used to determine the project that should be mutated.
    /// </summary>
    public string SourceProjectName { get; init; }

    /// <summary>
    /// When not empty, use these test projects to test the project under test.
    /// </summary>
    public IEnumerable<string> TestProjects { get; init; } = Enumerable.Empty<string>();

    /// <summary>
    /// Filters out tests in the project using the given expression.
    /// </summary>
    public string TestCaseFilter { get; init; }

    /// <summary>
    /// The reports that should be activated when stryker is done testing.
    /// </summary>
    public IEnumerable<Reporter> Reporters { get; init; } = Enumerable.Empty<Reporter>();

    /// <summary>
    /// When true, the baseline feature should be enabled.
    /// </summary>
    public bool WithBaseline { get; init; }

    /// <summary>
    /// When the baseline feature is enabled, this selects the source of the baseline.
    /// </summary>
    public BaselineProvider BaselineProvider { get; init; }

    /// <summary>
    /// The url to connect to the Azure File Storage API
    /// </summary>
    public string AzureFileStorageUrl { get; init; }

    /// <summary>
    /// The url to connect to the Azure File Storage API
    /// </summary>
    public string AzureFileStorageSas { get; init; }

    /// <summary>
    /// The url to connect to the dashboard API
    /// </summary>
    public string DashboardUrl { get; init; }

    /// <summary>
    /// The api key to connect to the dashboard API
    /// </summary>
    public string DashboardApiKey { get; init; }

    /// <summary>
    /// When true the since feature is enabled.
    /// </summary>
    public bool Since { get; init; }

    /// <summary>
    /// When using the since feature this commitish is used to get the changed files from git. Only changed files should be mutated.
    /// </summary>
    public string SinceTarget { get; init; }

    /// <summary>
    /// These files are ignored from detecting changes in test projects.
    /// Context: When using the since feature, all tests are run again if files in the test project change (as these could impact the test results)
    /// When the file is present in this option the tests should not run again as the file does not impact test results.
    /// </summary>
    public IEnumerable<FilePattern> DiffIgnoreChanges { get; init; } = Enumerable.Empty<FilePattern>();

    /// <summary>
    /// When no previous report can be found for the since feature, this commitish is used to se a baseline.
    /// </summary>
    public string FallbackVersion { get; init; }

    /// <summary>
    /// When publishing to the stryker dashboard, sets the project name on the dashboard.
    /// </summary>
    public string ModuleName { get; init; }

    /// <summary>
    /// When set to a value, reports should be opened by the filesystem after the reports have been generated.
    /// </summary>
    public ReportType? ReportTypeToOpen { get; init; }

    /// <summary>
    /// Files that should be mutated or ignored. Uses GLOB as pattern matching. Also parts of files can be ignored by format {10..21}
    /// </summary>
    public IEnumerable<FilePattern> Mutate { get; init; } = new[] { FilePattern.Parse("**/*") };

    /// <summary>
    /// Method call mutations that should not be tested. The implementation of the method may still be mutated and tested.
    /// </summary>
    public IEnumerable<Regex> IgnoredMethods { get; init; } = Enumerable.Empty<Regex>();

    /// <summary>
    /// Mutations that should not be tested.
    /// </summary>
    public IEnumerable<Mutator> ExcludedMutations { get; init; } = Enumerable.Empty<Mutator>();

    /// <summary>
    /// Linq expressions mutations that should not be tested.
    /// </summary>
    public IEnumerable<LinqExpression> ExcludedLinqExpressions { get; init; } = Enumerable.Empty<LinqExpression>();

    /// <summary>
    /// The optimization mode for coverage analysis for the current run.
    /// </summary>
    public OptimizationModes OptimizationMode { get; init; }

    /// <summary>
    /// This name is used in the dashboard report
    /// Is settable because this version can be detected by using DotNet.ReproducibleBuilds and thus can be overridden by stryker internally
    /// </summary>
    public string ProjectName
    {
        get => _projectName;
        set
        {
            _projectName = value;
            if (_parentOptions is not null)
            {
                _parentOptions.ProjectName = value;
            }
        }
    }

    /// <summary>
    /// This projectversion is used in the dashboard report
    /// Is settable because this version can be detected by using DotNet.ReproducibleBuilds and thus can be overridden by stryker internally
    /// </summary>
    public string ProjectVersion
    {
        get => _projectVersion;
        set
        {
            _projectVersion = value;
            if (_parentOptions is not null)
            {
                _parentOptions.ProjectVersion = value;
            }
        }
    }

    /// <summary>
    /// Instruct Stryker to break execution when at least one test failed on initial run.
    /// </summary>
    public bool BreakOnInitialTestFailure { get; set; }

    // Keep a reference on the parent instance in order to flow get/set properties (ProjectName and ProjectVersion) up to the parent
    // This is required for the dashboard reporter to work properly
    private StrykerOptions _parentOptions;
    private string _projectName;
    private string _projectVersion;

    public StrykerOptions Copy(string projectPath, string workingDirectory, string sourceProject, IEnumerable<string> testProjects) => new()
    {
        _parentOptions = this,
        AdditionalTimeout = AdditionalTimeout,
        AzureFileStorageSas = AzureFileStorageSas,
        AzureFileStorageUrl = AzureFileStorageUrl,
        BaselineProvider = BaselineProvider,
        ProjectPath = projectPath,
        WorkingDirectory = workingDirectory,
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
        ReportTypeToOpen = ReportTypeToOpen,
        Mutate = Mutate,
        MutationLevel = MutationLevel,
        OptimizationMode = OptimizationMode,
        OutputPath = OutputPath,
        ReportFileName = ReportFileName,
        ProjectName = ProjectName,
        SourceProjectName = sourceProject,
        ProjectVersion = ProjectVersion,
        Reporters = Reporters,
        SinceTarget = SinceTarget,
        SolutionPath = SolutionPath,
        TargetFramework = TargetFramework,
        TestProjects = testProjects,
        TestCaseFilter = TestCaseFilter,
        Thresholds = Thresholds,
        WithBaseline = WithBaseline,
        BreakOnInitialTestFailure = BreakOnInitialTestFailure,
    };
}
