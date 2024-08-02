using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Environment;
using Microsoft.Extensions.Logging;
using Stryker.Configuration.Exceptions;
using Stryker.Configuration.Initialisation.Buildalyzer;
using Stryker.Configuration.Logging;
using Stryker.Configuration;
using Stryker.Configuration.ProjectComponents.SourceProjects;
using Stryker.Configuration.ProjectComponents.TestProjects;
using Stryker.Configuration.Testing;

namespace Stryker.Configuration.Initialisation;

public interface IInputFileResolver
{
    IReadOnlyCollection<SourceProjectInfo> ResolveSourceProjectInfos(StrykerOptions options);
    IFileSystem FileSystem { get; }
}

/// <summary>
///  - Reads .csproj to find project under test
///  - Scans project under test and store files to mutate
///  - Build composite for files
/// </summary>
public class InputFileResolver : IInputFileResolver
{
    private readonly string[] _foldersToExclude = { "obj", "bin", "node_modules", "StrykerOutput" };
    private readonly ILogger _logger;
    private readonly IBuildalyzerProvider _analyzerProvider;
    private static readonly HashSet<string> importantProperties =
        ["Configuration", "Platform", "AssemblyName", "Configurations"];

    private readonly INugetRestoreProcess _nugetRestoreProcess;


    private readonly StringWriter _buildalyzerLog = new();

    public InputFileResolver(IFileSystem fileSystem,
        IBuildalyzerProvider analyzerProvider = null, INugetRestoreProcess nugetRestoreProcess = null, ILogger<InputFileResolver> logger = null)
    {
        FileSystem = fileSystem;
        _analyzerProvider = analyzerProvider ?? new BuildalyzerProvider();
        _nugetRestoreProcess = nugetRestoreProcess ?? new NugetRestoreProcess();
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
    }

    public InputFileResolver() : this(new FileSystem()) { }

    public IFileSystem FileSystem { get; }

    public IReadOnlyCollection<SourceProjectInfo> ResolveSourceProjectInfos(StrykerOptions options)
    {
        var manager = _analyzerProvider.Provide(options.SolutionPath, options.DevMode ? new AnalyzerManagerOptions { LogWriter = _buildalyzerLog }: null);
        if (options.IsSolutionContext)
        {
            var projectList = manager.Projects.Values.Select(p => p.ProjectFile.Path).ToList();
            _logger.LogInformation("Identifying projects to mutate in {solution}. This can take a while.", options.SolutionPath);

            return AnalyzeAndIdentifyProjects(projectList , options, manager, ScanMode.NoScan);
        }

        // we analyze the test project(s) and identify the project to be mutated
        List<string> testProjectFileNames;
        if (options.TestProjects != null && options.TestProjects.Any())
        {
            testProjectFileNames = options.TestProjects.Select(FindTestProject).ToList();
        }
        else
        {
            testProjectFileNames = [FindTestProject(options.ProjectPath)];
        }

        _logger.LogInformation("Analyzing {count} test project(s).", testProjectFileNames.Count);
        var result = AnalyzeAndIdentifyProjects(testProjectFileNames, options, manager, ScanMode.ScanTestProjectReferences);
        if (result.Count <= 1)
        {
            return result;
        }
        // Too many references found
        // look for one project that references all provided test projects
        result = result.Where(p => testProjectFileNames.TrueForAll(n => p.TestProjectsInfo.TestProjects.Any(t => t.ProjectFilePath == n))).ToList();
        if (result.Count == 1)
        {
            return result;
        }
        // still ambiguous
        var stringBuilder = new StringBuilder().AppendLine(
                "Test project contains more than one project reference. Please set the project option (https://stryker-mutator.io/docs/stryker-net/configuration#project-file-name) to specify which project to mutate.")
            .Append(BuildReferenceChoice(result.Select(p => p.AnalyzerResult.ProjectFilePath)));
        throw new InputException(stringBuilder.ToString());
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

    private List<SourceProjectInfo> AnalyzeAndIdentifyProjects(List<string> projectList, StrykerOptions options,
        IAnalyzerManager manager, ScanMode mode)
    {
        // build all projects
        if (!string.IsNullOrEmpty(options.Configuration))
        {
            manager.SetGlobalProperty("Configuration", options.Configuration);
        }
        _logger.LogDebug("Analyzing {count} projects.", manager.Projects.Count);

        // we match test projects to mutable projects
        var findMutableAnalyzerResults = FindMutableAnalyzerResults(AnalyzeAllNeededProjects(projectList, options, manager, mode));

        if (findMutableAnalyzerResults.Count == 0)
        {
            // no mutable project found
            throw new InputException("No project references found. Please add a project reference to your test project and retry.");
        }

        var analyzerResults = findMutableAnalyzerResults.Keys.GroupBy(p => p.ProjectFilePath).ToList();
        var projectInfos = new List<SourceProjectInfo>();
        foreach (var group in analyzerResults)
        {
            // we must select projects according to framework settings if any
            var analyzerResult = SelectAnalyzerResult(group, options.TargetFramework);

            projectInfos.Add(BuildSourceProjectInfo(options, analyzerResult, findMutableAnalyzerResults[analyzerResult]));
        }

        if (projectInfos.Count == 0)
        {
            _logger.LogError("Project analysis failed.");
            throw new InputException("No valid project analysis results could be found.");
        }
        return projectInfos;
    }

    private ConcurrentBag<(IAnalyzerResults result, bool isTest)> AnalyzeAllNeededProjects(List<string> projectList, StrykerOptions options, IAnalyzerManager manager, ScanMode mode)
    {
        var mutableProjectsAnalyzerResults = new ConcurrentBag<(IAnalyzerResults result, bool isTest)>();
        try
        {
            var list = new DynamicEnumerableQueue<string>(projectList);
            var normalizedProjectUnderTestNameFilter = !string.IsNullOrEmpty(options.SourceProjectName) ? options.SourceProjectName.Replace("\\", "/") : null;
            while(!list.Empty)
            {
                Parallel.ForEach(list.Consume(),
                    new ParallelOptions
                        { MaxDegreeOfParallelism = options.DevMode ? 1 : Math.Max(options.Concurrency, 1) }, projectFile =>
                    {
                        var project = manager.GetProject(projectFile);
                        var buildResult = AnalyzeSingleProject(project, options);
                        if (buildResult.Count == 0)
                        {
                            // analysis failed
                            return;
                        }
                        var isTestProject = buildResult.IsTestProject();
                        var referencesToAdd = ScanReferences(mode, buildResult);

                        var addProject = isTestProject || normalizedProjectUnderTestNameFilter == null ||
                                         project.ProjectFile.Path.Replace('\\', '/')
                                             .Contains(normalizedProjectUnderTestNameFilter, StringComparison.InvariantCultureIgnoreCase);
                        if (addProject)
                            mutableProjectsAnalyzerResults.Add((buildResult, isTestProject));
                        foreach (var reference in referencesToAdd)
                        {
                            list.Add(reference);
                        }
                    });
            }
        }
        catch (AggregateException ex)
        {
            throw ex.GetBaseException();
        }

        return mutableProjectsAnalyzerResults;
    }

    /// <summary>
    /// Scan the references of a project and add them for analysis according to scan option
    /// </summary>
    /// <param name="mode">scan mode</param>
    /// <param name="buildResult">analyzer results to parse</param>
    /// <returns>A list of project to analyse</returns>
    private List<string> ScanReferences(ScanMode mode, IAnalyzerResults buildResult)
    {
        var referencesToAdd = new List<string>();
        var isTestProject = buildResult.IsTestProject();

        if (mode == ScanMode.NoScan)
        {
            return referencesToAdd;
        }

        // Stryker will recursively scan projects
        // add any project reference to ease progressive discovery (when not using solution file)
        foreach (var projectReference in buildResult.SelectMany(p => p.ProjectReferences))
        {
            // in single level mode we only want to find the projects referenced by test project
            if (( mode == ScanMode.ScanTestProjectReferences && !isTestProject) || !FileSystem.File.Exists(projectReference))
            {
                continue;
            }

            referencesToAdd.Add(projectReference);
        }

        return referencesToAdd;
    }

    private IAnalyzerResults AnalyzeSingleProject(IProjectAnalyzer project, StrykerOptions options)
    {
        if (options.DevMode)
        {
            // clear the logs for the next project
            _buildalyzerLog.GetStringBuilder().Clear();
        }
        var projectLogName = Path.GetRelativePath(options.WorkingDirectory, project.ProjectFile.Path);
                    _logger.LogDebug("Analyzing {ProjectFilePath}", projectLogName);
        var buildResult = project.Build([options.TargetFramework]);

        var buildResultOverallSuccess = buildResult.OverallSuccess || Array.
            TrueForAll(project.ProjectFile.TargetFrameworks,tf =>
            buildResult.Any(br => IsValid(br) && br.TargetFramework == tf));

        if (!buildResultOverallSuccess)
        {
            if (options.DevMode)
            {
                // clear the logs to remove the noise
                _buildalyzerLog.GetStringBuilder().Clear();
            }
            // if this is a full framework project, we can retry after a nuget restore
            if (buildResult.Any(r => !IsValid(r) && r.TargetsFullFramework()))
            {
                _logger.LogWarning("Project {projectFilePath} analysis failed. Stryker will retry after a nuget restore.", projectLogName);
                _nugetRestoreProcess.RestorePackages(options.SolutionPath, options.MsBuildPath ?? buildResult.First().MsBuildPath());
            }
            var buildOptions = new EnvironmentOptions
            {
                Restore = true
            };
            // retry the analysis
            buildResult = project.Build([options.TargetFramework], buildOptions);

            // check the new status
            buildResultOverallSuccess = Array.TrueForAll(project.ProjectFile.TargetFrameworks,tf =>
                buildResult.Any(br => IsValid(br) && br.TargetFramework == tf));
        }
 
        if (buildResultOverallSuccess)
        {
            _logger.LogDebug("Analysis of project {projectFilePath} succeeded.", projectLogName);
            LogAnalyzerResult(buildResult, options);
            return buildResult;
        }
        var failedFrameworks = project.ProjectFile.TargetFrameworks.Where(tf =>
            !buildResult.Any(br => IsValid(br) && br.TargetFramework == tf)).ToList();
        _logger.LogWarning(
            "Analysis of project {projectFilePath} failed for frameworks {frameworkList}.",
            projectLogName, string.Join(',', failedFrameworks));

        if (options.DevMode)
        {
            _logger.LogWarning("Project analysis failed. The MsBuild log is below.");
            _logger.LogInformation(_buildalyzerLog.ToString());
        }

        // if there is no valid result, drop it altogether
        return buildResult.All(br => !IsValid(br)) ? new AnalyzerResults() : buildResult;
    }

    private void LogAnalyzerResult(IAnalyzerResults analyzerResults, StrykerOptions options)
    {
        // do not log if trace is not enabled
        if (!_logger.IsEnabled(LogLevel.Trace) || !options.DevMode)
        {
            return;
        }
        var log = new StringBuilder();
        // dump all properties as it can help diagnosing build issues for user project.
        log.AppendLine("**** Buildalyzer result ****");

        log.AppendLine($"Project: {analyzerResults.First().ProjectFilePath}");
        foreach(var analyzerResult in analyzerResults)
        {
            log.AppendLine($"TargetFramework: {analyzerResult.TargetFramework}" );
            log.AppendLine("Succeeded: {analyzerResult.Succeeded}");

            var properties = analyzerResult.Properties ?? new Dictionary<string, string>();
            foreach (var property in importantProperties)
            {
                log.AppendLine($"Property {property}={properties.GetValueOrDefault(property)??"\"'undefined'\""}");
            }
            foreach (var sourceFile in analyzerResult.SourceFiles ?? Enumerable.Empty<string>())
            {
                log.AppendLine($"SourceFile {sourceFile}");
            }

            foreach (var reference in analyzerResult.References ?? Enumerable.Empty<string>())
            {
                log.AppendLine($"References: {Path.GetFileName(reference)} (in {Path.GetDirectoryName(reference)})");
            }

            foreach (var property in properties)
            {
                if (importantProperties.Contains(property.Key)) continue; // already logged 
                log.AppendLine($"Property {property.Key}={property.Value.Replace(Environment.NewLine, "\\n")}");
            }
            log.AppendLine();
        }
        log.AppendLine("**** End Buildalyzer result ****");
        _logger.LogTrace(log.ToString());
    }

    public IAnalyzerResult SelectAnalyzerResult(IEnumerable<IAnalyzerResult> analyzerResults, string targetFramework)
    {
        var validResults = analyzerResults.Where(a => a.TargetFramework is not null).ToList();
        if (validResults.Count == 0)
        {
            throw new InputException("No valid project analysis results could be found.");
        }

        if (targetFramework is null)
        {
             // we try to avoid desktop versions
             return validResults.Find(a => a.Succeeded && !a.TargetsFullFramework()) ?? validResults[0];
        }

        var resultForRequestedFramework = validResults.Find(a => a.TargetFramework == targetFramework);
        if (resultForRequestedFramework is not null)
        {
            return resultForRequestedFramework;
        }

        var firstAnalyzerResult = validResults[0];
        var availableFrameworks = validResults.Select(a => a.TargetFramework).Distinct();
        var firstFramework = firstAnalyzerResult.TargetFramework;
        _logger.LogWarning(
            """
             Could not find a project analysis for the chosen target framework {0}.
             The available target frameworks are: {1}.
                  first available framework will be selected, which is {2}.
             """, targetFramework, string.Join(',', availableFrameworks), firstFramework);

        return firstAnalyzerResult;
    }

    // checks if an analyzer result is valid
    private static bool IsValid(IAnalyzerResult br) => br.Succeeded || (br.SourceFiles.Length > 0 && br.References.Length > 0 && br.TargetFramework !=null);

    private static Dictionary<IAnalyzerResult, List<IAnalyzerResult>> FindMutableAnalyzerResults(ConcurrentBag<(IAnalyzerResults result, bool isTest)> mutableProjectsAnalyzerResults)
    {
        var mutableToTestMap = new Dictionary<IAnalyzerResult, List<IAnalyzerResult>>();
        var analyzerTestProjects = mutableProjectsAnalyzerResults.Where(p => p.isTest).SelectMany(p=>p.result).Where(p => p.BuildsAnAssembly());
        var mutableProjects = mutableProjectsAnalyzerResults.Where(p => !p.isTest).SelectMany(p=>p.result).Where(p => p.BuildsAnAssembly()).ToArray();
        // for each test project
        foreach (var testProject in analyzerTestProjects)
        {
            // we identify which project are referenced by it
            foreach(var mutableProject in mutableProjects)
            {
                if (Array.TrueForAll( testProject.References,  r =>
                    !r.Equals(mutableProject.GetAssemblyPath(), StringComparison.OrdinalIgnoreCase) &&
                    !r.Equals(mutableProject.GetReferenceAssemblyPath(), StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                if (!mutableToTestMap.TryGetValue(mutableProject, out var dependencies))
                {
                    dependencies = [];
                    mutableToTestMap[mutableProject] = dependencies;
                }
                dependencies.Add(testProject);
            }
        }

        return mutableToTestMap;
    }

    /// <summary>
    /// Builds a <see cref="SourceProjectInfo"/> instance describing a project its associated test project(s)
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <param name="analyzerResult">project buildalyzer result</param>
    /// <param name="analyzerResults">test project(s) buildalyzer result(s)</param>
    /// <returns></returns>
    private SourceProjectInfo BuildSourceProjectInfo(StrykerOptions options,
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

        var builder = (ProjectComponentsBuilder)(language switch
        {
            Language.Csharp => new CsharpProjectComponentsBuilder(
                targetProjectInfo,
                options,
                _foldersToExclude,
                _logger,
                FileSystem),

            Language.Fsharp => new FsharpProjectComponentsBuilder(
                targetProjectInfo,
                options,
                _foldersToExclude,
                _logger,
                FileSystem),
            _ => throw new NotSupportedException($"Language not supported: {language}")
        });

        var inputFiles = builder.Build();
        builder.InjectHelpers(inputFiles);
        targetProjectInfo.OnProjectBuilt = builder.PostBuildAction();
        targetProjectInfo.ProjectContents = inputFiles;

        _logger.LogInformation("Found project {0} to mutate.", analyzerResult.ProjectFilePath);
        targetProjectInfo.TestProjectsInfo = new(FileSystem)
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
            builder.Append("  ").AppendLine(projectReference.Replace("\\", "/"));
        }
        return builder;
    }


    private sealed class DynamicEnumerableQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly HashSet<T> _cache;

        public DynamicEnumerableQueue(IEnumerable<T> init)
        {
            _cache = [..init];
            _queue = new Queue<T>(_cache);
        }

        public bool Empty => _queue.Count == 0;

        public void Add(T entry)
        {
            if (!_cache.Add(entry))
            {
                return;
            }
            _queue.Enqueue(entry);
        }

        public IEnumerable<T> Consume()
        {
            while(_queue.Count > 0)
            {
                yield return _queue.Dequeue();
            }
        }
    }
}
