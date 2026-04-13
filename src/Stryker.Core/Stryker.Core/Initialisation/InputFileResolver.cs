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
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Solutions;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

public interface IInputFileResolver
{
    RelatedSourceProjectsInfo ResolveSourceProjectInfos(IStrykerOptions options);
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
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IBuildalyzerProvider _analyzerProvider = analyzerProvider ?? throw new ArgumentNullException(nameof(analyzerProvider));
    private readonly ISolutionProvider _solutionProvider = solutionProvider ?? throw new ArgumentNullException(nameof(solutionProvider));

    private readonly INugetRestoreProcess _nugetRestoreProcess = nugetRestoreProcess ?? throw new ArgumentNullException(nameof(nugetRestoreProcess));
    public IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    /// <summary>
    /// Identifies the project(s) to mutate and their associated test project(s) according to provided options, and returns a collection of <see cref="SourceProjectInfo"/> describing them.
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <returns>a collection of <see cref="SourceProjectInfo"/> describing mutable project</returns>
    /// <exception cref="InputException">Thrown if the method fails during analysis.</exception>
    public RelatedSourceProjectsInfo ResolveSourceProjectInfos(IStrykerOptions options)
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
                return new RelatedSourceProjectsInfo(BuildTracker(options, null), [], _logger);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogCritical(e, "Failed to access solution file {SolutionFile}.", options.SolutionPath);
                return new RelatedSourceProjectsInfo(BuildTracker(options, null), [], _logger);
            }
            catch (AggregateException e) // Handles exceptions from .Result on Task
            {
                _logger.LogCritical(e, "Failed to load solution file {SolutionFile}.", options.SolutionPath);
                return new RelatedSourceProjectsInfo(BuildTracker(options, null), [], _logger);
            }
        }

        var solutionInfo = BuildTracker(options, solution);
        var sourceProjectInfos =  options.IsSolutionContext ? FindProjectsInSolutionMode(options, solutionInfo, normalizedProjectUnderTestNameFilter)
            : FindProjectInTargetProjectMode(options, solutionInfo, normalizedProjectUnderTestNameFilter);
        return new RelatedSourceProjectsInfo(solutionInfo, sourceProjectInfos, _logger);
    }

    private ProjectsTracker BuildTracker(IStrykerOptions options, SolutionFile? solution) =>
        new(solution, options, _analyzerProvider, _nugetRestoreProcess, FileSystem, _logger)
            { TargetFramework = options.TargetFramework };

    private IReadOnlyCollection<SourceProjectInfo> FindProjectsInSolutionMode(IStrykerOptions options, ProjectsTracker solutionInfo,
        string normalizedProjectUnderTestNameFilter)
    {
        _logger.LogInformation("Identifying projects to mutate in {Solution}. This can take a while.",
            FileSystem.Path.GetFileNameWithoutExtension(solutionInfo.SolutionFilePath));

        // analyze all projects
        solutionInfo.SelectAllProjects();
        _logger.LogDebug("Analyzing {ProjectsCount} projects.", solutionInfo.ProjectCount);
        // we analyze every project in the solution
        var mutableProjectsAnalyzerResults = AnalyzeAllNeededProjects(solutionInfo,
            normalizedProjectUnderTestNameFilter,
            options,
            ScanMode.NoScan);
        // we identify target projects and their associated test projects
        var (findMutableAnalyzerResults, orphanedProjects) =
            ExtractMutableProjectTrees(mutableProjectsAnalyzerResults);
        // we keep only suitable candidates
        return AnalyzeAndIdentifyProjects(options, findMutableAnalyzerResults, orphanedProjects);
    }

    /// <summary>
    /// Identifies the project(s) to mutate and their associated test project(s) according to provided options, and returns a collection of <see cref="SourceProjectInfo"/> describing them, when not mutating
    /// the whole solution.
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <param name="solution">solution file if any</param>
    /// <param name="normalizedProjectUnderTestNameFilter">name filter to apply to the mutated projects</param>
    /// <returns>identified mutable projects matching the provided name filter (when provided). Can be empty</returns>
    private List<SourceProjectInfo> FindProjectInTargetProjectMode(IStrykerOptions options, ProjectsTracker solution,
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
        var analyzeAllNeededProjects = AnalyzeAllNeededProjects(solution,
            normalizedProjectUnderTestNameFilter, options, ScanMode.ScanTestProjectReferences);
        // we match test projects to mutable projects
        var (findMutableAnalyzerResults, orphans) = ExtractMutableProjectTrees(analyzeAllNeededProjects);

        var result = AnalyzeAndIdentifyProjects(options, findMutableAnalyzerResults, orphans);
        return SelectSingleProject(normalizedProjectUnderTestNameFilter, result, targetProjectMode, testProjectFileNames);
    }

    private List<SourceProjectInfo> SelectSingleProject(string normalizedProjectUnderTestNameFilter, List<SourceProjectInfo> result, bool targetProjectMode,
        List<string> testProjectFileNames)
    {
        switch (result.Count)
        {
            case 1:
                return result;
            case 0:
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
            default:
            {
                // Too many references found
                // look for one project that references all provided test projects
                result =
                [
                    .. result.Where(p =>
                        testProjectFileNames.TrueForAll(n =>
                            p.TestProjectsInfo.TestProjects.Any(t => t.ProjectFilePath == n)))
                ];
                if (result.Count == 1)
                {
                    _logger.LogInformation(
                        "Selected project {ProjectFile} as it is referenced by all provided test projects.",
                        result[0].AnalyzerResult.ProjectFilePath);
                    return result;
                }

                // still ambiguous
                var stringBuilder = new StringBuilder().AppendLine(
                        "Multiple projects identified as potential candidate for mutation testing. Please set the project option (https://stryker-mutator.io/docs/stryker-net/configuration#project-file-name) to specify which project to mutate.")
                    .Append(BuildReferenceChoice(result.Select(p => p.AnalyzerResult.ProjectFilePath)));
                throw new InputException(stringBuilder.ToString());
            }
        }
    }

    public string FindTestProject(string path)
    {
        var projectFile = FindProjectFile(path);
        _logger.LogDebug("Using {Filename} as test project", projectFile);
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
        List<MutableProjectTree> findMutableAnalyzerResults,
        List<ProjectSimulatedBuildHandler> unusedTestProjects)
    {
        // build all projects
        _logger.LogDebug("Scanning {Count} possible targets.", findMutableAnalyzerResults.Count);

        var suitableCandidates =
            findMutableAnalyzerResults.Where(p => p.IsValidTarget).ToList();

        // do we have at least one valid target?
        if (suitableCandidates.Count == 0)
        {
            // no mutable project found
            LogAnalysis(findMutableAnalyzerResults, unusedTestProjects, options.DiagMode);
            throw new InputException("Failed to analyze project builds. Stryker cannot continue.");
        }

        // we keep only on target framework per project
        foreach (var candidate in suitableCandidates)
        {
            candidate.KeepOnlyOneTarget(options.TargetFramework);
        }
        // keep only projects with one or more test projects
        // we must select projects according to framework settings if any
        var projectInfos = suitableCandidates.Where(p => p.Targets.Count > 0)
            .Select(analyzerResult => analyzerResult.Targets[0].BuildSourceProjectInfo(options, FileSystem))
            .ToList();

        if (projectInfos.Count != 0)
        {
            return projectInfos;
        }

        _logger.LogError("Project analysis failed.");
        throw new InputException("No valid project analysis results could be found.");
    }

    // Log the analysis results
    private void LogAnalysis(List<MutableProjectTree> findMutableAnalyzerResults,
        List<ProjectSimulatedBuildHandler> unusedTestProjects, bool optionsDiagMode)
    {
        if (findMutableAnalyzerResults.Count == 0)
        {
            _logger.LogWarning( optionsDiagMode ? "No project found, check settings and ensure project file is not corrupted.":
                               """
                               No project found, check settings and ensure project file is not corrupted.
                               Use --diag option to have the simulated build logs.
                               """);
            return;
        }
        foreach (var projectTree in findMutableAnalyzerResults)
        {
            projectTree.DumpForAnalysis();
        }
        // dump test projects that do not reference any mutable project
        foreach (var unusedTestProject in unusedTestProjects)
        {
            _logger.LogInformation("Test project {ProjectName} does not appear to test any mutable project, simulated build {Result}.",
                unusedTestProject.ProjectFileName,
                unusedTestProject.HasValidResults() ? "succeeded" : "failed");
        }

        if (!optionsDiagMode)
        {
            _logger.LogWarning("Use --diag option to have the analysis logs in the log file.");
        }
    }

    private ConcurrentBag<ProjectSimulatedBuildHandler> AnalyzeAllNeededProjects(
        ProjectsTracker solutionInfo,
        string normalizedProjectUnderTestNameFilter,
        IStrykerOptions options, ScanMode mode)
    {
        var mutableProjectsAnalyzerResults = new ConcurrentBag<ProjectSimulatedBuildHandler>();

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
                        var projectAnalysisContext = solutionInfo.GetProjectAnalysisContext(entry);

                        IEnumerable<IAnalyzerResult> buildResult = AnalyzeSingleProject(projectAnalysisContext, options);

                        // apply project name filter (except for test projects)
                        if (!(normalizedProjectUnderTestNameFilter == null
                              || buildResult.IsTestProject()
                              || projectAnalysisContext.ProjectFileName.Replace('\\', '/')
                              .Contains(normalizedProjectUnderTestNameFilter,
                                  StringComparison.InvariantCultureIgnoreCase)))
                        {
                            return;
                        }

                        mutableProjectsAnalyzerResults.Add(projectAnalysisContext);
                        // recursively scan dependencies only if enabled and current project is a test project
                        if (mode == ScanMode.NoScan
                            || (mode == ScanMode.ScanTestProjectReferences && !projectAnalysisContext.IsTestProject()))
                        {
                            return;
                        }

                        // scan references if recursive scan is enabled
                        // Stryker will recursively scan projects
                        // add any project reference for progressive discovery (when not using solution file)
                        list.Add(projectAnalysisContext.GetProjectReferences()
                            .Where(projectReference => FileSystem.File.Exists(projectReference)));
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

    private IAnalyzerResults AnalyzeSingleProject(ProjectSimulatedBuildHandler project, IStrykerOptions options)
    {
        var projectLogName = FileSystem.Path.GetRelativePath(options.WorkingDirectory, project.ProjectFileName);
        var shouldConfirmSuccess = false;
        _logger.LogDebug("Analyzing {ProjectFilePath}", projectLogName);

        var buildResult = project.Analyze();
        var buildResultOverallSuccess = project.HasValidResults();

        // if buildalyzer failed, we can try again with a nuget restore, as missing packages is a common cause of
        // buildalyzer failure, especially for full framework projects
        if (buildResult.All(ar=>!ar.Succeeded))
        {
            if (project.IsNetFramework && Environment.OSVersion.Platform!=PlatformID.Win32NT)
            {
                _logger.LogWarning("Project {ProjectFilePath} is a .NET Framework project. It requires Windows for mutation testing. Discarding.", projectLogName);
                return buildResult;
            }

            shouldConfirmSuccess = true;
            _logger.LogWarning("Project {ProjectFilePath} simulated build failed. Trying again with a nuget restore.", projectLogName);

            // if this is a full framework project, we can retry after a nuget restore
            buildResult = project.Analyze(withRestore: true);

            // check the new status
            buildResultOverallSuccess = project.HasValidResults();

            if (!buildResultOverallSuccess && !string.IsNullOrEmpty(options.TargetFramework))
            {
                // still failed, we can try using target framework option
                _logger.LogWarning("Project {ProjectFilePath} simulated build failed again. Last attempt, forcing the target framework.", projectLogName);
                buildResult = project.Analyze(forceFramework: true);
                buildResultOverallSuccess = project.HasValidResults();
            }
        }
        if (!buildResult.OverallSuccess)
        {
            _logger.LogWarning("Project {ProjectFilePath} simulated build failed. Use '--diag' option to have the build log.", projectLogName);
        }

        if (buildResultOverallSuccess)
        {
            _logger.Log(shouldConfirmSuccess ? LogLevel.Warning : LogLevel.Debug,
                "Analysis of project {ProjectFilePath} succeeded{Extra}", projectLogName,
                buildResult.OverallSuccess ? "." : " but simulated build failed; Stryker may fail later.");
            return buildResult;
        }

        // log failure details
        _logger.LogWarning(
            "Analysis of project {ProjectFilePath} failed for frameworks {FrameworkList}.",
            projectLogName, string.Join(',', project.FailedFrameworks));

        if (options.DiagMode)
        {
            _logger.LogWarning("{ProjectFilePath}'s build log is: {Log}", projectLogName, project.LastBuildLog);
        }

        return buildResult;
    }

    private (List<MutableProjectTree>, List<ProjectSimulatedBuildHandler>) ExtractMutableProjectTrees(
        IEnumerable<ProjectSimulatedBuildHandler> mutableProjectsAnalyzerResults)
    {
        // separate test projects from mutable projects, and keep only analyzer results building an assembly (exclude solution folders and such)
        var testProjects = new List<ProjectSimulatedBuildHandler>();
        var mutableProjects = new List<ProjectSimulatedBuildHandler>();
        foreach (var project in mutableProjectsAnalyzerResults)
        {
            if (project.IsTestProject())
            {
                testProjects.Add(project);
            }
            else if (project.BuildsAnAssembly())
            {
                mutableProjects.Add(project);
            }
        }
        var mutableToTestMap = mutableProjects.ToDictionary(p =>p, p => new MutableProjectTree(p, _logger));
        var unusedTestProjects = new List<ProjectSimulatedBuildHandler>();

        // for each test project
        foreach (var testProject in testProjects)
        {
            if (ScanAssemblyReferences(mutableToTestMap, mutableProjects, testProject))
            {
                continue;
            }

            _logger.LogInformation("Could not find an assembly reference to a mutable assembly for project {ProjectName}. Will look into project references.", testProject.ProjectFileName);
            // we try to find a project reference
            if (!ScanProjectReferences(mutableToTestMap, mutableProjects, testProject))
            {
                unusedTestProjects.Add(testProject);
            }
        }

        return (mutableToTestMap.Values.ToList(), unusedTestProjects);
    }

    private static bool ScanAssemblyReferences(Dictionary<ProjectSimulatedBuildHandler, MutableProjectTree> mutableToTestMap,
        List<ProjectSimulatedBuildHandler> mutableProjects, ProjectSimulatedBuildHandler testProject)
    {
        if (testProject.AnalyzerLastResults.Count == 0)
        {
            throw new InvalidOperationException($"You must analyze the test project {testProject.ProjectFileName} before trying to find its references.");
        }
        var foundOneProject = false;
        // we do the work for each available target
        foreach (var variant in testProject.AnalyzerLastResults)
        {
            // we analyze references
            foreach (var variantReference in variant.References)
            {
                foreach (var candidateProject in mutableProjects)
                {
                    if (!candidateProject.FindMatchingVariant(variantReference, out var candidateTarget))
                    {
                        continue;
                    }
                    // find the entry
                    mutableToTestMap[candidateProject][candidateTarget].TestProjects.Add(variant);

                    foundOneProject = true;
                }
            }
        }
        return foundOneProject;
    }

    private static bool ScanProjectReferences(Dictionary<ProjectSimulatedBuildHandler, MutableProjectTree> mutableToTestMap,
        List<ProjectSimulatedBuildHandler> mutableProjects, ProjectSimulatedBuildHandler testProject)
    {
        if (testProject.AnalyzerLastResults.Count == 0)
        {
            throw new InvalidOperationException($"You must analyze the test project {testProject.ProjectFileName} before trying to find its references.");
        }
        var foundOneProject = false;
        foreach (var variant in testProject.AnalyzerLastResults)
        {
            foreach (var projectReference in variant.ProjectReferences)
            {
                var candidateProject = mutableProjects.FirstOrDefault(p => p.ProjectFileName == projectReference);
                var candidateProjectVariants = candidateProject?.AnalyzerLastResults;
                if (candidateProjectVariants == null)
                {
                    // probably another test project
                    continue;
                }
                // we try to find a target with the same target framework or one of the same kind (full or core
                var candidateVariant = candidateProjectVariants.FirstOrDefault( v=> v.TargetFramework == variant.TargetFramework) ??
                                       candidateProjectVariants.FirstOrDefault( v=> v.TargetsFullFramework() == variant.TargetsFullFramework());
                if (candidateVariant == null)
                {
                    continue;
                }
                mutableToTestMap[candidateProject][candidateVariant].TestProjects.Add(variant);

                foundOneProject = true;
            }
        }

        return foundOneProject;
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

        public void Add(IEnumerable<T> entries)
        {
            foreach (var entry in entries)
            {
                if (!_cache.TryAdd(entry, true))
                {
                    continue;
                }
                _queue.Enqueue(entry);
            }
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
