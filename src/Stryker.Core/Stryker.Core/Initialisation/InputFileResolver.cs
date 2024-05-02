using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Testing;
using static FSharp.Compiler.Symbols.FSharpImplementationFileDeclaration;

namespace Stryker.Core.Initialisation;

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
    private readonly INugetRestoreProcess _nugetRestoreProcess;

    private readonly StringWriter _buildalyzerLog = new();

    public InputFileResolver(IFileSystem fileSystem, IBuildalyzerProvider analyzerProvider = null, INugetRestoreProcess nugetRestoreProcess = null
        , ILogger<InputFileResolver> logger = null)
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
        if (options.IsSolutionContext)
        {
            return AnalyzeSolution(options);
        }
        List<string> testProjectFileNames;
        if (options.TestProjects != null && options.TestProjects.Any())
        {
            testProjectFileNames = options.TestProjects.Select(FindTestProject).ToList();
        }
        else
        {
            testProjectFileNames = [FindTestProject(options.ProjectPath)];
        }
        var manager = _analyzerProvider.Provide(options.SolutionPath, new AnalyzerManagerOptions { LogWriter = _buildalyzerLog });
        foreach(var project in testProjectFileNames)
        {
            manager.GetProject(project);
        }
        return AnalyzeAndIdentifyProjects(options, manager, ScanMode.SingleLevelScan);
    }

    private enum ScanMode
    {
        NoScan = 0,
        SingleLevelScan = 1
    }

    private List<SourceProjectInfo> AnalyzeSolution(StrykerOptions options)
    {
        _logger.LogInformation("Identifying projects to mutate in {0}. This can take a while.", options.SolutionPath);
        var manager = _analyzerProvider.Provide(options.SolutionPath, options.DevMode ? new AnalyzerManagerOptions { LogWriter = _buildalyzerLog }: null);

        return AnalyzeAndIdentifyProjects(options, manager, ScanMode.NoScan);
    }

    private List<SourceProjectInfo> AnalyzeAndIdentifyProjects(StrykerOptions options, IAnalyzerManager manager, ScanMode mode)
    {
        var list = new DynamicEnumerableQueue<IProjectAnalyzer>(manager.Projects.Values);
        // build all projects
        var testProjectsAnalyzerResults = new ConcurrentDictionary<string, IAnalyzerResults>();
        var mutableProjectsAnalyzerResults = new ConcurrentDictionary<string, IAnalyzerResults>();
        if (!string.IsNullOrEmpty(options.Configuration))
        {
            manager.SetGlobalProperty("Configuration", options.Configuration);
        }
        _logger.LogDebug("Analyzing {count} projects.", manager.Projects.Count);
        try
        {
            var normalizedProjectUnderTestNameFilter = !string.IsNullOrEmpty(options.SourceProjectName) ? options.SourceProjectName.Replace("\\", "/") : null;
            while(!list.Empty)
            {
                Parallel.ForEach(list.Consume(),
                    new ParallelOptions
                        { MaxDegreeOfParallelism = options.DevMode ? 1 : Math.Max(options.Concurrency, 1) }, project =>
                    {
                        if (options.DevMode)
                        {
                            // clear the logs for the next project
                            _buildalyzerLog.GetStringBuilder().Clear();
                        }
                        var projectLogName = Path.GetRelativePath(options.WorkingDirectory, project.ProjectFile.Path);
                        _logger.LogDebug("Analyzing {projectFilePath}", projectLogName);
                        var buildResult = project.Build([options.TargetFramework]);

                        // if this is a full framework project, we can retry after a nuget restore
                        if (!buildResult.OverallSuccess && buildResult.TargetsFullFramework())
                        {
                            _logger.LogWarning("Project {projectFilePath} targets full framework. Stryker will retry after a nuget restore.", projectLogName);
                            _nugetRestoreProcess.RestorePackages (project.ProjectFile.Path);
                            buildResult = project.Build([options.TargetFramework]);
                        }

                        var buildResultOverallSuccess = buildResult.OverallSuccess;

                        if (!buildResultOverallSuccess && Array.TrueForAll(project.ProjectFile.TargetFrameworks,tf =>
                                buildResult.Any(br => IsValid(br) && br.TargetFramework == tf)))
                        {
                            // if all expected frameworks are built, we consider the build a success
                            buildResultOverallSuccess = true;
                        }

                        if (buildResultOverallSuccess)
                        {
                            _logger.LogDebug("Analysis of project {projectFilePath} succeeded.", projectLogName);
                            if (options.DevMode)
                            {
                                LogAnalyzerResult(buildResult.First());
                            }
                        }
                        else
                        {
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
                        }

                        var isTestProject = buildResult.First().IsTestProject();
                        var isProjectMatch = !isTestProject && (normalizedProjectUnderTestNameFilter == null ||
                                                                project.ProjectFile.Path.Replace('\\', '/')
                                                                    .Contains(normalizedProjectUnderTestNameFilter));
                        if (mode != ScanMode.NoScan)
                        {
                            // Stryker will recursively scan projects
                            // add any project reference to ease progressive discovery (when not using solution file)
                            foreach (var projectReference in buildResult.SelectMany(p => p.ProjectReferences))
                            {
                                // in single level mode we only want to find the referenced test project
                                if (mode == ScanMode.SingleLevelScan && (!isTestProject || (normalizedProjectUnderTestNameFilter != null &&
                                        !projectReference.Replace('\\', '/')
                                        .Contains(normalizedProjectUnderTestNameFilter))))
                                {
                                    continue;
                                }

                                if (FileSystem.File.Exists(projectReference))
                                {
                                    list.Add(manager.GetProject(projectReference));
                                }
                            }
                        }

                        // tests projects are always added
                        if (isTestProject)
                            testProjectsAnalyzerResults[project.ProjectFile.Name] = buildResult;
                        // we only consider any matching normal project
                        else if (isProjectMatch)
                            mutableProjectsAnalyzerResults[project.ProjectFile.Name] = buildResult;
                    });
            }
        }
        catch (AggregateException ex)
        {
            throw ex.GetBaseException();
        }

        // we match test projects to mutable projects
        var findMutableAnalyzerResults = FindMutableAnalyzerResults(testProjectsAnalyzerResults, mutableProjectsAnalyzerResults);

        var analyzerResults = findMutableAnalyzerResults.Keys.GroupBy(p => p.ProjectFilePath).ToList();
        var projectInfos = new List<SourceProjectInfo>();
        foreach (var group in analyzerResults)
        {
            // we must select projects according to framework settings if any
            var analyzerResult = SelectAnalyzerResult(group, options.TargetFramework);
            if (analyzerResult == null)
            {
                continue;
            }
            projectInfos.Add(BuildSourceProjectInfo(options, analyzerResult, findMutableAnalyzerResults[analyzerResult]));
        }
        if (projectInfos.Count == 0)
        {
            _logger.LogError("Project analysis failed.");
            throw new InputException("No valid project analysis results could be found.");
        }
        return projectInfos;
    }

    private static readonly HashSet<string> importantProperties =
        ["Configuration", "Platform", "AssemblyName", "Configurations"];

    private void LogAnalyzerResult(IAnalyzerResult analyzerResult)
    {
        // do not log if trace is not enabled
        if (!_logger.IsEnabled(LogLevel.Trace))
        {
            return;
        }
        var log = new StringBuilder();
        // dump all properties as it can help diagnosing build issues for user project.
        log.AppendLine("**** Buildalyzer result ****");

        log.AppendLine($"Project: {analyzerResult.ProjectFilePath}");
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

        log.AppendLine("**** Buildalyzer result ****");
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
            // any framework will do
            return validResults[0];
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

    private static Dictionary<IAnalyzerResult, List<IAnalyzerResult>> FindMutableAnalyzerResults(ConcurrentDictionary<string, IAnalyzerResults> testProjectsAnalyzerResults,
        ConcurrentDictionary<string, IAnalyzerResults> mutableProjectsAnalyzerResults)
    {
        // first pass: identify all projects tested by each test project
        var mutableToTestMap = new Dictionary<IAnalyzerResult, List<IAnalyzerResult>>();
        foreach (var testProject in
                 testProjectsAnalyzerResults.SelectMany(p=>p.Value).Where(p => p.BuildsAnAssembly()))
        {
            var realMutableProjects = mutableProjectsAnalyzerResults.SelectMany(p=>p.Value).Where(p => p.BuildsAnAssembly());
            foreach(var mutableProject in realMutableProjects)
            {
                if (Array.TrueForAll( testProject.References,  r =>
                    !r.Equals(mutableProject.GetAssemblyPath(), StringComparison.OrdinalIgnoreCase) &&
                    !r.Equals(mutableProject.GetReferenceAssemblyPath(), StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                if (!mutableToTestMap.ContainsKey(mutableProject))
                {
                    mutableToTestMap[mutableProject] = new();
                }
                mutableToTestMap[mutableProject].Add(testProject);
            }
        }

        return mutableToTestMap;
    }

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

        var builder = GetProjectComponentBuilder(language, options, targetProjectInfo);
        var inputFiles = builder.Build();
        targetProjectInfo.ProjectContents = inputFiles;

        _logger.LogInformation("Found project {0} to mutate.", analyzerResult.ProjectFilePath);
        targetProjectInfo.TestProjectsInfo = new(FileSystem)
        {
            TestProjects = analyzerResults.Select(testProjectAnalyzerResult => new TestProject(FileSystem, testProjectAnalyzerResult)).ToList()
        };
        return targetProjectInfo;
    }

    public string FindTestProject(string path)
    {
        var projectFile = FindProjectFile(path);
        _logger.LogDebug("Using {0} as test project", projectFile);
        return projectFile;
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
            projectFiles = FileSystem.Directory.GetFiles(path, "*.*").Where(file => file.EndsWith("csproj", StringComparison.OrdinalIgnoreCase) || file.EndsWith("fsproj", StringComparison.OrdinalIgnoreCase)).ToArray();
        }
        catch (DirectoryNotFoundException)
        {
            throw new InputException($"No .csproj or .fsproj file found, please check your project directory at {path}");
        }

        _logger.LogTrace("Scanned the directory {0} for {1} files: found {2}", path, "*.csproj", projectFiles);

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

    public string FindSourceProject(IEnumerable<IAnalyzerResult> testProjects, StrykerOptions options)
    {
        var projectReferences = FindProjectsReferencedByAllTestProjects(testProjects.ToList());
        var sourceProjectPath = string.IsNullOrEmpty(options?.SourceProjectName) ? DetermineTargetProjectWithoutNameFilter(projectReferences) : DetermineSourceProjectWithNameFilter(options, projectReferences);

        _logger.LogDebug("Using {0} as project under test", sourceProjectPath);

        return sourceProjectPath;
    }

    internal string DetermineSourceProjectWithNameFilter(StrykerOptions options, IReadOnlyCollection<string> projectReferences)
    {
        var stringBuilder = new StringBuilder();

        var normalizedProjectUnderTestNameFilter = options.SourceProjectName.Replace("\\", "/");
        var projectReferencesMatchingNameFilter = projectReferences
            .Where(x => x.Replace("\\", "/")
                .Contains(normalizedProjectUnderTestNameFilter, StringComparison.OrdinalIgnoreCase)).ToImmutableList();

        var count = projectReferencesMatchingNameFilter.Count;
        if (count == 1)
        {
            return projectReferencesMatchingNameFilter.Single();
        }

        if (count == 0)
        {
            stringBuilder.Append("No project reference matched the given project filter ")
                .Append($"'{options.SourceProjectName}'");
        }
        else
        {
            stringBuilder.Append("More than one project reference matched the given project filter ")
                .Append($"'{options.SourceProjectName}'")
                .AppendLine(", please specify the full name of the project reference.");

        }

        stringBuilder.Append(BuildReferenceChoice(projectReferences));

        throw new InputException(stringBuilder.ToString());
    }

    private static string DetermineTargetProjectWithoutNameFilter(IReadOnlyCollection<string> projectReferences)
    {
        var count = projectReferences.Count;
        if (count == 1)
        {
            return projectReferences.Single();
        }

        var stringBuilder = new StringBuilder();
        if (count > 1) // Too many references found
        {
            stringBuilder.AppendLine("Test project contains more than one project reference. Please set the project option (https://stryker-mutator.io/docs/stryker-net/configuration#project-file-name) to specify which project to mutate.")
                .Append(BuildReferenceChoice(projectReferences));
        }
        else  // No references found
        {
            stringBuilder.AppendLine("No project references found. Please add a project reference to your test project and retry.");

        }
        throw new InputException(stringBuilder.ToString());
    }

    private static IReadOnlyCollection<string> FindProjectsReferencedByAllTestProjects(IReadOnlyCollection<IAnalyzerResult> testProjects)
    {
        var allProjectReferences = testProjects.SelectMany(t => t.ProjectReferences);
        var projectReferences = allProjectReferences.GroupBy(x => x).Where(g => g.Count() == testProjects.Count).Select(g => g.Key).ToImmutableList();
        return projectReferences;
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

    private ProjectComponentsBuilder GetProjectComponentBuilder(
        Language language,
        StrykerOptions options,
        SourceProjectInfo projectInfo) => language switch
    {
        Language.Csharp => new CsharpProjectComponentsBuilder(
            projectInfo,
            options,
            _foldersToExclude,
            _logger,
            FileSystem),

        Language.Fsharp => new FsharpProjectComponentsBuilder(
            projectInfo,
            options,
            _foldersToExclude,
            _logger,
            FileSystem),
        _ => throw new NotSupportedException($"Language not supported: {language}")
    };

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
