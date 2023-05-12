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

namespace Stryker.Core.Initialisation
{
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
        private readonly IProjectFileReader _projectFileReader;
        private readonly ILogger _logger;

        public InputFileResolver(IFileSystem fileSystem, IProjectFileReader projectFileReader, ILogger<InputFileResolver> logger = null)
        {
            FileSystem = fileSystem;
            _projectFileReader = projectFileReader ?? new ProjectFileReader();
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
        }

        public InputFileResolver() : this(new FileSystem(), new ProjectFileReader()) { }

        public IFileSystem FileSystem { get; }

        public IReadOnlyCollection<SourceProjectInfo> ResolveSourceProjectInfos(StrykerOptions options)
        {
            if (!options.IsSolutionContext)
            {
                List<string> testProjectFileNames;
                if (options.TestProjects != null && options.TestProjects.Any())
                {
                    testProjectFileNames = options.TestProjects.Select(FindTestProject).ToList();
                }
                else
                {
                    testProjectFileNames = new List<string> {FindTestProject(options.ProjectPath) };
                }

                var testProjects = testProjectFileNames.Select(testProjectFile => _projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath, options.TargetFramework)).ToList();

                var analyzerResult = _projectFileReader.AnalyzeProject(FindSourceProject(testProjects, options),
                    options.SolutionPath, options.TargetFramework);
                return new[]
                {
                    BuildSourceProjectInfo(options, analyzerResult, testProjects)
                };
            }

            // Analyze source project
            // Analyze all projects in the solution with buildalyzer
            var solutionAnalyzerResults = AnalyzeSolution(options);
            var solutionTestProjects = solutionAnalyzerResults.Where(p => p.IsTestProject()).ToList();
            var projectsUnderTestAnalyzerResult = solutionAnalyzerResults.Where(p => !p.IsTestProject()).ToList();
            _logger.LogInformation("Found {0} source projects", projectsUnderTestAnalyzerResult.Count);
            _logger.LogInformation("Found {0} test projects", solutionTestProjects.Count);

            var dependents = FindDependentProjects(projectsUnderTestAnalyzerResult);
            if (!string.IsNullOrEmpty(options.SourceProjectName))
            {
                var normalizedProjectUnderTestNameFilter = options.SourceProjectName.Replace("\\", "/");
                projectsUnderTestAnalyzerResult = projectsUnderTestAnalyzerResult.Where(p =>
                    p.ProjectFilePath.Replace('\\', '/').Contains(normalizedProjectUnderTestNameFilter)).ToList();
            }
            return BuildProjectInfos(options, dependents, projectsUnderTestAnalyzerResult, solutionTestProjects);
        }

        private static Dictionary<string, HashSet<string>> FindDependentProjects(IReadOnlyCollection<IAnalyzerResult> projectsUnderTest)
        {
            // need to scan traverse dependencies
            // dependents contains the list of projects depending on each (non test) projects
            var dependents = projectsUnderTest.ToDictionary(p=>p.ProjectFilePath, p => new HashSet<string>(new []{p.ProjectFilePath}));
            // register explicit dependencies
            foreach (var result in projectsUnderTest)
            {
                foreach (var reference in result.ProjectReferences)
                {
                    dependents[reference].Add(result.ProjectFilePath);
                }
            }
            
            // we need to dig recursively to find recursive dependencies, until none are discovered
            bool foundNewDependency;
            do
            {
                var nextDependence = new Dictionary<string, HashSet<string>>();
                foundNewDependency = false;
                foreach (var (project, dependent) in dependents)
                {
                    var newList = new HashSet<string>(dependent);
                    foreach (var sub in dependent.Where(sub => dependents.ContainsKey(sub)))
                    {
                        newList.UnionWith(dependents[sub]);
                    }

                    foundNewDependency = foundNewDependency || newList.Count > dependent.Count;
                    nextDependence[project] = newList;
                }
                dependents = nextDependence;
            } while (foundNewDependency);

            return dependents;
        }

        private List<IAnalyzerResult> AnalyzeSolution(StrykerOptions options)
        {
            _logger.LogInformation("Identifying projects to mutate in {0}. This can take a while.",  options.SolutionPath);
            var manager = _projectFileReader.AnalyzeSolution(options.SolutionPath);

            // build all projects
            var projectsAnalyzerResults = new ConcurrentBag<IAnalyzerResult>();
            _logger.LogDebug("Analyzing {count} projects.", manager.Projects.Count);
            try
            {
                Parallel.ForEach(manager.Projects.Values, project =>
                {
                    var projectLogName = Path.GetRelativePath(options.WorkingDirectory, project.ProjectFile.Path);
                    _logger.LogDebug("Analyzing {projectFilePath}", projectLogName);
                    var buildResult = project.Build();
                    var projectAnalyzerResult = buildResult.Results.FirstOrDefault();
                    if (projectAnalyzerResult is not null)
                    {
                        projectsAnalyzerResults.Add(projectAnalyzerResult);
                        _logger.LogDebug("Analysis of project {projectFilePath} succeeded.", projectLogName);
                    }
                    else
                    {
                        _logger.LogWarning("Analysis of project {projectFilePath} failed.", projectLogName);
                    }
                });
            }
            catch (AggregateException ex)
            {
                throw ex.GetBaseException();
            }

            return projectsAnalyzerResults.ToList();
        }

        private IReadOnlyCollection<SourceProjectInfo> BuildProjectInfos(StrykerOptions options,
            IReadOnlyDictionary<string, HashSet<string>> dependents,
            IReadOnlyCollection<IAnalyzerResult> projectsUnderTestAnalyzerResult,
            IReadOnlyCollection<IAnalyzerResult> testProjects)
        {
            var result = new List<SourceProjectInfo>(projectsUnderTestAnalyzerResult.Count);
            foreach (var project in projectsUnderTestAnalyzerResult)
            {
                var projectLogName = Path.GetRelativePath(Path.GetDirectoryName(options.SolutionPath), project.ProjectFilePath);
                var analyzerResultsForTestProjects = testProjects
                    .Where(testProject => testProject.ProjectReferences.Any(reference => dependents[project.ProjectFilePath].Contains(reference)));
                var relatedTestProjects = analyzerResultsForTestProjects.Select(p =>p.ProjectFilePath).ToList();
                if (relatedTestProjects.Count > 0)
                {
                    _logger.LogDebug("Matched {0} to {1} test projects:", projectLogName, relatedTestProjects.Count);

                    foreach (var relatedTestProjectAnalyzerResults in relatedTestProjects)
                    {
                        _logger.LogDebug("{0}", relatedTestProjectAnalyzerResults);
                    }

                    var sourceProject = BuildSourceProjectInfo(options, project, analyzerResultsForTestProjects);

                    result.Add(sourceProject);
                }
                else
                {
                    _logger.LogWarning("Project {0} will not be mutated because Stryker did not find a test project for it.", projectLogName);
                }
            }
            return result;
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

            if (projectFiles.Length > 1)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Expected exactly one .csproj file, found more than one:");
                foreach (var file in projectFiles)
                {
                    sb.AppendLine(file);
                }
                sb.AppendLine();
                sb.AppendLine("Please specify a test project name filter that results in one project.");
                throw new InputException(sb.ToString());
            }

            if (!projectFiles.Any())
            {
                throw new InputException($"No .csproj or .fsproj file found, please check your project or solution directory at {path}");
            }
            _logger.LogTrace("Found project file {file} in path {path}", projectFiles.Single(), path);

            return projectFiles.Single();
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

        private string DetermineTargetProjectWithoutNameFilter(IReadOnlyCollection<string> projectReferences)
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
                    _foldersToExclude,
                    _logger,
                    FileSystem),
                _ => throw new NotSupportedException()
            };
    }
}
