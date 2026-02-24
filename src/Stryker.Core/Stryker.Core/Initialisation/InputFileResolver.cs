using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Solutions;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

public interface IInputFileResolver
{
    IReadOnlyCollection<SourceProjectInfo> ResolveSourceProjectInfos(IStrykerOptions options);
    IFileSystem FileSystem { get; }
}

/// <summary>
///  - Reads .csproj to find project under test
///  - Scans project under test and store files to mutate
///  - Build composite for files
/// </summary>
public class InputFileResolver(
    IFileSystem fileSystem,
    IBuildalyzerProvider analyzerProvider,
    INugetRestoreProcess nugetRestoreProcess,
    ISolutionProvider solutionProvider,
    ILogger<InputFileResolver> logger)
    : IInputFileResolver
{
    private readonly string[] _foldersToExclude = ["obj", "bin", "node_modules", "StrykerOutput"];
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IBuildalyzerProvider _analyzerProvider = analyzerProvider ?? throw new ArgumentNullException(nameof(analyzerProvider));
    private readonly ISolutionProvider _solutionProvider = solutionProvider ?? throw new ArgumentNullException(nameof(solutionProvider));
    private static readonly HashSet<string> ImportantProperties =
        ["Configuration", "Platform", "AssemblyName", "Configurations", "TargetPath", "OS"];

    private readonly INugetRestoreProcess _nugetRestoreProcess = nugetRestoreProcess ?? throw new ArgumentNullException(nameof(nugetRestoreProcess));
    private readonly Dictionary<string, string> _buildLogs = [];

    public IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    /// <summary>
    /// Identifies the project(s) to mutate and their associated test project(s) according to provided options, and returns a collection of <see cref="SourceProjectInfo"/> describing them.
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <returns>a collection of <see cref="SourceProjectInfos"/> describing mutable project</returns>
    /// <exception cref="InputException">Thrown if the method fails during analysis.</exception>
    public IReadOnlyCollection<SourceProjectInfo> ResolveSourceProjectInfos(IStrykerOptions options)
    {
        var normalizedProjectUnderTestNameFilter = NormalizePath(options.SourceProjectName);

        SolutionFile solution;
        if (string.IsNullOrEmpty(options.SolutionPath))
        {
            solution = null;
        }
        else
        {
            // load the solution file when provided
            try
            {
                _logger.LogDebug("Loading solution file {SolutionFile}.", options.SolutionPath);
                solution = _solutionProvider.GetSolution(options.SolutionPath);
            }
            catch (IOException e)
            {
                _logger.LogCritical(e, "Failed to load solution file {SolutionFile}.", options.SolutionPath);
                return [];
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogCritical(e, "Failed to access solution file {SolutionFile}.", options.SolutionPath);
                return [];
            }
            catch (AggregateException e) // Handles exceptions from .Result on Task
            {
                _logger.LogCritical(e, "Failed to load solution file {SolutionFile}.", options.SolutionPath);
                return [];
            }
        }

        var solutionInfo = new TargetsForMutation(solution, options, _analyzerProvider ,_logger, _nugetRestoreProcess) { TargetFramework = options.TargetFramework };
        if (options.IsSolutionContext)
        {
            return FindProjectsInSolutionMode(options, solutionInfo, normalizedProjectUnderTestNameFilter);
        }

        var result = SourceProjectInfos(options, solutionInfo, normalizedProjectUnderTestNameFilter);
        if (result.Count<=1)
        {
            return result;
        }
        // still ambiguous
        var stringBuilder = new StringBuilder().AppendLine(
                "Test project contains more than one project reference. Please set the project option (https://stryker-mutator.io/docs/stryker-net/configuration#project-file-name) to specify which project to mutate.")
            .Append(BuildReferenceChoice(result.Select(p => p.AnalyzerResult.ProjectFilePath)));
        throw new InputException(stringBuilder.ToString());
    }

    /// <summary>
    /// Identifies the project(s) to mutate and their associated test project(s) according to provided options, and returns a collection of <see cref="SourceProjectInfo"/> describing them, when not mutating
    /// the whole solution.
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <param name="solution">solution file if any</param>
    /// <param name="normalizedProjectUnderTestNameFilter">name filter to apply to the mutated projects</param>
    /// <returns>identified mutable projects matching the provided name filter (when provided). Can be empty</returns>
    private List<SourceProjectInfo> SourceProjectInfos(IStrykerOptions options, TargetsForMutation solution,
        string normalizedProjectUnderTestNameFilter)
    {
        // we analyze the test project(s) and identify the project to be mutated
        var testProjectsSpecified = options.TestProjects.Any();
        var testProjectFileNames = testProjectsSpecified ? options.TestProjects.Select(FindTestProject).ToList()
            : [FindTestProject(options.ProjectPath)];

        _logger.LogInformation("Analyzing {ProjectCount} test project(s).", testProjectFileNames.Count);
        // if test project is provided but no source project => working directory should contain the project to mutate, we try to detect
        // it and use it as a filter for the analysis. This allows users to just cd into a project and run stryker
        var targetProjectMode = testProjectsSpecified && string.IsNullOrEmpty(options.SourceProjectName);
        if (targetProjectMode)
        {
            _logger.LogDebug("Assume working directory contains target project to be mutated.");
            normalizedProjectUnderTestNameFilter = NormalizePath(FindProjectFile(options.WorkingDirectory));
            targetProjectMode =
                options.TestProjects.All(tp => NormalizePath(tp) != normalizedProjectUnderTestNameFilter);
            if (!targetProjectMode)
            {
                // we detected a test project, discard it
                // Stryker will need to detect mutable projects out from test project references
                _logger.LogDebug("Working directory contains a test project.");
                normalizedProjectUnderTestNameFilter = null;
            }
        }
        solution.AddProjects(testProjectFileNames);
        // we analyze test projects
        var analyzeAllNeededProjects = AnalyzeAllNeededProjects(solution, normalizedProjectUnderTestNameFilter, options, ScanMode.ScanTestProjectReferences);
        // we match test projects to mutable projects
        var (findMutableAnalyzerResults, orphans) = FindMutableAnalyzerResults(analyzeAllNeededProjects);

        var result = AnalyzeAndIdentifyProjects(options, solution, findMutableAnalyzerResults, orphans);
        var mutableProjectsFound = result.Count;
        if (mutableProjectsFound == 1)
        {
            return result;
        }

        if (mutableProjectsFound == 0)
        {
            if (targetProjectMode)
            {
                _logger.LogError("Project {ProjectFile} could not be found as a project referenced by the provided test projects.", normalizedProjectUnderTestNameFilter);
            }
            else
            {
                _logger.LogError("No project could be found as a project referenced by the provided test projects.");
            }

            return result;
        }

        // Too many references found
        // look for one project that references all provided test projects
        result = [.. result.Where(p => testProjectFileNames.TrueForAll(n => p.TestProjectsInfo.TestProjects.Any(t => t.ProjectFilePath == n)))];
        if (result.Count == 1)
        {
            _logger.LogInformation("Selected project {ProjectFile} as it is referenced by all provided test projects.", result[0].AnalyzerResult.ProjectFilePath);
        }

        return result;
    }

    private IReadOnlyCollection<SourceProjectInfo> FindProjectsInSolutionMode(IStrykerOptions options, TargetsForMutation solutionInfo,
        string normalizedProjectUnderTestNameFilter)
    {
        _logger.LogInformation("Stryker will mutate solution {Solution}.", FileSystem.Path.GetFileNameWithoutExtension(options.SolutionPath));
        _logger.LogInformation("Identifying projects to mutate in {Solution}. This can take a while.", options.SolutionPath);

        // analyze all projects
        solutionInfo.SelectAllProjects();
       _logger.LogDebug("Analyzing {0} projects.", solutionInfo.ProjectCount);
        // we match test projects to mutable projects
        var mutableProjectsAnalyzerResults = AnalyzeAllNeededProjects(solutionInfo,
            normalizedProjectUnderTestNameFilter,
            options,
            ScanMode.NoScan);
        var (findMutableAnalyzerResults, orphanedProjects) =
            FindMutableAnalyzerResults(mutableProjectsAnalyzerResults);

        solutionInfo.SelectAllProjects();
        return AnalyzeAndIdentifyProjects(options, solutionInfo, findMutableAnalyzerResults, orphanedProjects);
    }

    public string FindTestProject(string path)
    {
        var projectFile = FindProjectFile(path);
        _logger.LogDebug("Using {0} as test project", projectFile);
        return projectFile;
    }

    private enum ScanMode
    {
        NoScan = 0, // no project added during analysis
        ScanTestProjectReferences = 1 // add test project references during scan
    }

    // analyze projects, do same for their upstream dependencies if activated, and identify which one(s)
    // to proceed with
    private List<SourceProjectInfo> AnalyzeAndIdentifyProjects(IStrykerOptions options,
        TargetsForMutation solutionInfo,
        Dictionary<IAnalyzerResult, List<IAnalyzerResult>> findMutableAnalyzerResults,
        List<IAnalyzerResult> unusedTestProjects)
    {
        // build all projects
        _logger.LogDebug("Analyzing {Count} projects.", findMutableAnalyzerResults.Count);

        // we match test projects to mutable projects
        if (findMutableAnalyzerResults.All(r =>
                !r.Key.IsValid() || r.Value.All(r2 => !r2.IsValid())))
        {
            // no mutable project found
            LogAnalysis(findMutableAnalyzerResults, unusedTestProjects, options.DiagMode);
            throw new InputException("Failed to analyze project builds. Stryker cannot continue.");
        }

        // keep only projects with one or more test projects
        var analyzerResults = findMutableAnalyzerResults
            .Where(p => p.Value.Count > 0)
            .Select(p => p.Key).GroupBy(p => p.ProjectFilePath);
        // we must select projects according to framework settings if any
        var projectInfos = analyzerResults
            .Select(g => SelectAnalyzerResult(g, options.TargetFramework))
            .Select(analyzerResult => BuildSourceProjectInfo(options, solutionInfo, analyzerResult, findMutableAnalyzerResults[analyzerResult]))
            .ToList();

        if (projectInfos.Count != 0)
        {
            return projectInfos;
        }

        _logger.LogError("Project analysis failed.");
        throw new InputException("No valid project analysis results could be found.");
    }

    // Log the analysis results
    private void LogAnalysis(Dictionary<IAnalyzerResult, List<IAnalyzerResult>> findMutableAnalyzerResults,
        List<IAnalyzerResult> unusedTestProjects, bool optionsDiagMode)
    {
        if (findMutableAnalyzerResults.Count == 0)
        {
            _logger.LogWarning("""
                               No project found, check settings and ensure project file is not corrupted.
                               Use --diag option to have the analysis logs in the log file.
                               """);
            return;
        }
        foreach (var (mutableProject, testProjects) in findMutableAnalyzerResults)
        {
            _logger.LogInformation("Project {ProjectPath} analysis {Result}.",
                mutableProject.ProjectFilePath,
                mutableProject.IsValid() ? "succeeded" : "failed hence can't be mutated");
            if (testProjects.Count == 0)
            {
                _logger.LogWarning("  can't be mutated because no test project references it. If this is a test project, " +
                                   "ensure it has the property: <IsTestProject>true</IsTestProject> in its project file.");
                continue;
            }
            // dump associated test projects
            foreach (var testProject in testProjects)
            {
                _logger.LogInformation("  referenced by test project {ProjectName}, analysis {Result}.",
                    testProject.ProjectFilePath,
                    testProject.IsValid() ? "succeeded" : "failed");
            }
            // provide synthetic status
            if (testProjects.Any(r => r.IsValid()))
            {
                _logger.LogInformation("  can be mutated.");
            }
            else
            {
                _logger.LogWarning("  can't be mutated because all referencing test projects' analysis failed.");
            }
        }
        // dump test projects that do not reference any mutable project
        foreach (var unusedTestProject in unusedTestProjects)
        {
            _logger.LogInformation("Test project {ProjectName} does not appear to test any mutable project, analysis {Result}.",
                unusedTestProject.ProjectFilePath,
                unusedTestProject.IsValid() ? "succeeded" : "failed");
        }

        if (!optionsDiagMode)
        {
            _logger.LogWarning("Use --diag option to have the analysis logs in the log file.");
        }
    }

    private ConcurrentBag<(IEnumerable<IAnalyzerResult> result, bool isTest)> AnalyzeAllNeededProjects(
        TargetsForMutation solutionInfo,
        string normalizedProjectUnderTestNameFilter,
        IStrykerOptions options, ScanMode mode)
    {
        var mutableProjectsAnalyzerResults = new ConcurrentBag<(IEnumerable<IAnalyzerResult> result, bool isTest)>();

        var list = new DynamicEnumerableQueue<string>(solutionInfo.SelectedProjects);
        try
        {
            var parallelOptions = new ParallelOptions
                { MaxDegreeOfParallelism = options.DiagMode ? 1 : Math.Max(options.Concurrency, 1) };
            while (!list.Empty)
            {
                Parallel.ForEach(list.Consume(),
                    parallelOptions, entry =>
                    {
                        var buildResult = AnalyzeThisProject(
                            solutionInfo.GetProjectAnalysisContext(entry),
                            normalizedProjectUnderTestNameFilter,
                            options,
                            mutableProjectsAnalyzerResults);
                        // scan references if recursive scan is enabled
                        ScanReferences(mode, buildResult).ForEach(p => list.Add(p));
                    }
                );
            }
        }
        catch (AggregateException ex)
        {
            throw ex.GetBaseException();
        }

        return mutableProjectsAnalyzerResults;
    }

    private IEnumerable<IAnalyzerResult> AnalyzeThisProject(ProjectAnalyzerContext project,
        string normalizedProjectUnderTestNameFilter,
        IStrykerOptions options,
        ConcurrentBag<(IEnumerable<IAnalyzerResult> result, bool isTest)> mutableProjectsAnalyzerResults)
    {
        IEnumerable<IAnalyzerResult> buildResult = AnalyzeSingleProject(project, options);
        if (!buildResult.Any())
        {
            // analysis failed
            mutableProjectsAnalyzerResults.Add((buildResult, false));
            return buildResult;
        }

        var isTestProject = buildResult.IsTestProject();
        if (isTestProject)
        {
            // filter frameworks for test projects (if one is selected)
            buildResult = [SelectAnalyzerResult(buildResult, options.TargetFramework)];
        }

        // apply project name filter (except for test projects)
        if (isTestProject || normalizedProjectUnderTestNameFilter == null ||
            project.ProjectFileName.Replace('\\', '/')
                .Contains(normalizedProjectUnderTestNameFilter,
                    StringComparison.InvariantCultureIgnoreCase))
        {
            mutableProjectsAnalyzerResults.Add((buildResult, isTestProject));
        }

        return buildResult;
    }

    /// <summary>
    /// Scan the references of a project and add them for analysis according to scan option
    /// </summary>
    /// <param name="mode">scan mode</param>
    /// <param name="buildResult">analyzer results to parse</param>
    /// <returns>A list of project to analyze</returns>
    private List<string> ScanReferences(ScanMode mode, IEnumerable<IAnalyzerResult> buildResult)
    {
        var referencesToAdd = new List<string>();

        if (mode == ScanMode.NoScan || (mode == ScanMode.ScanTestProjectReferences && !buildResult.IsTestProject()))
        {
            return referencesToAdd;
        }

        // Stryker will recursively scan projects
        // add any project reference for progressive discovery (when not using solution file)
        referencesToAdd.AddRange(buildResult.SelectMany(p => p.ProjectReferences).Where(projectReference => FileSystem.File.Exists(projectReference)));

        return referencesToAdd;
    }

    private IAnalyzerResults AnalyzeSingleProject(ProjectAnalyzerContext project, IStrykerOptions options)
    {
        var projectLogName = FileSystem.Path.GetRelativePath(options.WorkingDirectory, project.ProjectFileName);
        _logger.LogDebug("Analyzing {ProjectFilePath}", projectLogName);

        var buildResult = project.Analyze();
        _buildLogs[projectLogName] = project.LastBuildLog;
        var buildResultOverallSuccess = project.HasValidResults();

        // retry if it failed
        if (!buildResultOverallSuccess)
        {
            if (options.DiagMode)
            {
                _logger.LogWarning("Project {ProjectFilePath} analysis failed. The MsBuild log is: {Log}", projectLogName, _buildLogs[projectLogName]);
            }

            // if this is a full framework project, we can retry after a nuget restore
            buildResult = RetryBuild(project, options, out buildResultOverallSuccess);
            // store the build log
            _buildLogs[projectLogName] = project.LastBuildLog.ToString();
            // clear the log
        }

        LogAnalyzerResult(buildResult, options);
        if (buildResultOverallSuccess)
        {
            _logger.LogDebug("Analysis of project {projectFilePath} succeeded.", projectLogName);
            return buildResult;
        }

        // log failure details
        _logger.LogWarning(
            "Analysis of project {ProjectFilePath} failed for frameworks {FrameworkList}.",
            projectLogName, string.Join(',', project.FailedFrameworks));

        if (options.DiagMode)
        {
            _logger.LogWarning("Project {ProjectFilePath} analysis failed. The MsBuild log is: {Log}", projectLogName, _buildLogs[projectLogName]);
        }

        return buildResult;
    }


    private IAnalyzerResults RetryBuild(ProjectAnalyzerContext analyzer, IStrykerOptions options, out bool buildResultOverallSuccess)
    {
        var buildResult = analyzer.Analyze(withRestore: true);

        // check the new status
        buildResultOverallSuccess = buildResult.HasValidResult();

        if (buildResultOverallSuccess || string.IsNullOrEmpty(options.TargetFramework))
        {
            return buildResult;
        }

        // still failed, we can try using target framework option
        buildResult = analyzer.Analyze(forceFramework: true);
        buildResultOverallSuccess = buildResult.Any( br => br.IsValidFor(options.TargetFramework));

        return buildResult;
    }

    private void LogAnalyzerResult(IAnalyzerResults analyzerResults, IStrykerOptions options)
    {
        // do not log if trace is not enabled
        if (!_logger.IsEnabled(LogLevel.Trace) || !options.DiagMode)
        {
            return;
        }
        if (analyzerResults.Count == 0)
        {
            _logger.LogTrace("No analyzer results to log. This indicates an early failure in analysis, check build log for details.");
            return;
        }
        var log = new StringBuilder();
        // dump all properties as it can help diagnosing build issues for user project.
        log.AppendLine("**** Buildalyzer result ****");

        log.AppendLine($"Project: {analyzerResults.First().ProjectFilePath}");
        foreach (var analyzerResult in analyzerResults)
        {
            log.AppendLine($"TargetFramework: {analyzerResult.TargetFramework}");
            log.AppendLine($"Succeeded: {analyzerResult.Succeeded}");
            log.AppendLine($"Compiler command: {analyzerResult.Command}");

            var properties = analyzerResult.Properties;
            foreach (var property in ImportantProperties)
            {
                log.AppendLine($"Property {property}={properties.GetValueOrDefault(property) ?? "\"'undefined'\""}");
            }
            if (analyzerResult.SourceFiles.Length == 0)
            {
                log.AppendLine("** No source files identified **");
            }
            else
            {
                foreach (var sourceFile in analyzerResult.SourceFiles)
                {
                    log.AppendLine($"SourceFile {sourceFile}");
                }
            }
            if (analyzerResult.References.Length == 0)
            {
                log.AppendLine("** No references Identified **");
            }
            else
            {
                foreach (var reference in analyzerResult.References)
                {
                    log.AppendLine($"References: {FileSystem.Path.GetFileName(reference)} (in {FileSystem.Path.GetDirectoryName(reference)})");
                }
            }

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                // dumps all other properties as well, as they can be useful for diagnosing build issues
                foreach (var property in properties.Where( p => !ImportantProperties.Contains(p.Key) ))
                {
                    log.AppendLine($"Property {property.Key}={property.Value.Replace(Environment.NewLine, "\\n")}");
                }
            }

            log.AppendLine();
        }
        log.AppendLine("**** End Buildalyzer result ****");
        _logger.LogDebug(log.ToString());
    }

    private IAnalyzerResult SelectAnalyzerResult(IEnumerable<IAnalyzerResult> analyzerResults, string targetFramework)
    {
        var validResults = analyzerResults.ToList();
        var projectName = analyzerResults.First().ProjectFilePath;
        if (validResults.Count == 0)
        {
            throw new InputException($"No valid project analysis results could be found for '{projectName}'.");
        }

        if (targetFramework is null)
        {
            // we try to avoid desktop versions
            return PickFrameworkVersion();
        }

        var resultForRequestedFramework = validResults.Find(a => a.TargetFramework == targetFramework);
        if (resultForRequestedFramework is not null)
        {
            return resultForRequestedFramework;
        }
        // if there is only one available framework version, we log an info
        if (validResults.Count == 1)
        {
            var singleAnalyzerResult = validResults[0];
            _logger.LogInformation(
                "Could not find a valid analysis for target {0} for project '{1}'. Selected version is {2}.",
                targetFramework, projectName, singleAnalyzerResult.TargetFramework);
            return singleAnalyzerResult;
        }

        var firstAnalyzerResult = PickFrameworkVersion();
        var availableFrameworks = validResults.Select(a => a.TargetFramework).Distinct();
        _logger.LogWarning(
            """
             Could not find a valid analysis for target {0} for project '{1}'.
             The available target frameworks are: {2}.
                  selected version is {3}.
             """, targetFramework, projectName, string.Join(',', availableFrameworks), firstAnalyzerResult.TargetFramework);

        return firstAnalyzerResult;

        IAnalyzerResult PickFrameworkVersion()
        {
            return validResults.Find(a => a.IsValid() && !a.TargetsFullFramework()) ?? validResults[0];
        }
    }

    private (Dictionary<IAnalyzerResult, List<IAnalyzerResult>>, List<IAnalyzerResult>) FindMutableAnalyzerResults(ConcurrentBag<(IEnumerable<IAnalyzerResult> result, bool isTest)> mutableProjectsAnalyzerResults)
    {
        // separate test projects from mutable projects, and keep only analyzer results building an assembly (exclude solution folders and such)
        var analyzerTestProjects = mutableProjectsAnalyzerResults.Where(p => p.isTest).SelectMany(p => p.result).Where(p => p.BuildsAnAssembly());
        var mutableProjects = mutableProjectsAnalyzerResults.Where(p => !p.isTest).SelectMany(p => p.result).Where(p => p.BuildsAnAssembly()).ToArray();

        var mutableToTestMap = mutableProjects.ToDictionary(p => p, _ => new List<IAnalyzerResult>());
        var unusedTestProjects = new List<IAnalyzerResult>();
        // for each test project
        foreach (var testProject in analyzerTestProjects)
        {
            if (ScanAssemblyReferences(mutableToTestMap, mutableProjects, testProject))
            {
                continue;
            }

            _logger.LogInformation("Could not find an assembly reference to a mutable assembly for project {ProjectName}. Will look into project references.", testProject.ProjectFilePath);
            // we try to find a project reference
            if (!ScanProjectReferences(mutableToTestMap, mutableProjects, testProject))
            {
                unusedTestProjects.Add(testProject);
            }
        }

        return (mutableToTestMap, unusedTestProjects);
    }

    private static bool ScanProjectReferences(Dictionary<IAnalyzerResult, List<IAnalyzerResult>> mutableToTestMap,
        IAnalyzerResult[] mutableProjects, IAnalyzerResult testProject)
    {
        var mutableProject = mutableProjects.FirstOrDefault(p => testProject.ProjectReferences.Contains(p.ProjectFilePath));
        if (mutableProject == null)
        {
            return false;
        }
        if (!mutableToTestMap.TryGetValue(mutableProject, out var dependencies))
        {
            mutableToTestMap[mutableProject] = dependencies = [];
        }

        dependencies.Add(testProject);
        return true;
    }

    private static bool ScanAssemblyReferences(Dictionary<IAnalyzerResult, List<IAnalyzerResult>> mutableToTestMap,
        IAnalyzerResult[] mutableProjects, IAnalyzerResult testProject)
    {
        var foundOneProject = false;
        // we identify which projects are referenced by it
        foreach (var mutableProject in mutableProjects)
        {
            var assemblyPath = mutableProject.GetAssemblyPath();
            var refAssemblyPath = mutableProject.GetReferenceAssemblyPath();

            if (Array.TrueForAll(testProject.References, r => !r.Equals(assemblyPath, StringComparison.OrdinalIgnoreCase) &&
                                    !r.Equals(refAssemblyPath, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }
            if (!mutableToTestMap.TryGetValue(mutableProject, out var dependencies))
            {
                mutableToTestMap[mutableProject] = dependencies = [];
            }
            dependencies.Add(testProject);
            foundOneProject = true;
        }

        return foundOneProject;
    }

    /// <summary>
    /// Builds a <see cref="SourceProjectInfo"/> instance describing a project its associated test project(s)
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <param name="solutionInfo"></param>
    /// <param name="analyzerResult">project buildalyzer result</param>
    /// <param name="analyzerResults">test project(s) buildalyzer result(s)</param>
    /// <returns></returns>
    private SourceProjectInfo BuildSourceProjectInfo(IStrykerOptions options,
        TargetsForMutation solutionInfo,
        IAnalyzerResult analyzerResult,
        IEnumerable<IAnalyzerResult> analyzerResults)
    {
        var targetProjectInfo = new SourceProjectInfo
        {
            AnalyzerResult = analyzerResult
        };

        var language = targetProjectInfo.AnalyzerResult.GetLanguage();
        if (language == Language.Fsharp)
        {
            _logger.LogError(
                targetProjectInfo.LogError(
                    "Mutation testing of F# projects is not ready yet. No mutants will be generated."));
        }

        ProjectComponentsBuilder builder = language == Language.Csharp
            ? new CsharpProjectComponentsBuilder(targetProjectInfo, options, _foldersToExclude, _logger,
                FileSystem)
            : throw new NotSupportedException($"Language not supported: {language}");

        var inputFiles = builder.Build();
        builder.InjectHelpers(inputFiles);
        targetProjectInfo.OnProjectBuilt = builder.PostBuildAction();
        targetProjectInfo.ProjectContents = inputFiles;
        targetProjectInfo.TargetsForMutation = solutionInfo;
        _logger.LogInformation("Found project {ProjectFileName} to mutate.", analyzerResult.ProjectFilePath);
        targetProjectInfo.TestProjectsInfo = new TestProjectsInfo(FileSystem)
        {
            TestProjects = analyzerResults.Select(testProjectAnalyzerResult => new TestProject(FileSystem, testProjectAnalyzerResult)).ToList()
        };
        return targetProjectInfo;
    }

    private string FindProjectFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(path, "Project path cannot be null or empty.");
        }

        if (FileSystem.File.Exists(path) && (FileSystem.Path.HasExtension(".csproj") || FileSystem.Path.HasExtension(".fsproj")))
        {
            return path;
        }

        string[] projectFiles;
        try
        {
            projectFiles = FileSystem.Directory.GetFiles(path, "*.*?sproj").Where(file => file.EndsWith("csproj", StringComparison.OrdinalIgnoreCase) || file.EndsWith("fsproj", StringComparison.OrdinalIgnoreCase)).ToArray();
        }
        catch (DirectoryNotFoundException)
        {
            throw new InputException($"No .csproj or .fsproj file found, please check your project directory at {path}");
        }

        _logger.LogTrace("Scanned the directory {Path} for *.csproj files: found {ProjectFilesCount}", path, projectFiles);

        switch (projectFiles.Length)
        {
            case > 1:
            {
                var sb = new StringBuilder();
                sb.AppendLine("Expected exactly one .csproj file, found more than one:");
                foreach (var file in projectFiles)
                {
                    sb.AppendLine(file);
                }
                sb.AppendLine().AppendLine("Please specify a test project name filter that results in one project.");
                throw new InputException(sb.ToString());
            }
            case 0:
                throw new InputException($"No .csproj or .fsproj file found, please check your project or solution directory at {path}");
            default:
                _logger.LogTrace("Found project file {file} in path {path}", projectFiles.Single(), path);
                return projectFiles.Single();
        }
    }

    private static StringBuilder BuildReferenceChoice(IEnumerable<string> projectReferences)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Choose one of the following references:").AppendLine("");

        foreach (var projectReference in projectReferences)
        {
            builder.Append("  ").AppendLine(NormalizePath(projectReference));
        }
        return builder;
    }

    private static string NormalizePath(string path) => path?.Replace('\\', '/');

    private static string NormalizePlatform(string platform) =>
        string.Equals(platform, "Any CPU", StringComparison.OrdinalIgnoreCase) ? "AnyCPU" : platform;

    private sealed class DynamicEnumerableQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly ConcurrentDictionary<T, bool> _cache;

        public DynamicEnumerableQueue(IEnumerable<T> init)
        {
            _cache = new ConcurrentDictionary<T, bool>(init.ToDictionary(x => x, _ => true));
            _queue = new ConcurrentQueue<T>(_cache.Keys);
        }

        public bool Empty => _queue.IsEmpty;

        public void Add(T entry)
        {
            if (!_cache.TryAdd(entry, true))
            {
                return;
            }
            _queue.Enqueue(entry);
        }

        public IEnumerable<T> Consume()
        {
            while (!_queue.IsEmpty)
            {
                if (_queue.TryDequeue(out var entry))
                {
                    yield return entry;
                }
            }
        }
    }
}
