using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.Initialisation
{
    public interface IProjectOrchestrator
    {
        IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters, ITestRunner runner = null);
    }
    
    public class ProjectOrchestrator : IProjectOrchestrator
    {
        private readonly ILogger _logger;
        private readonly IBuildalyzerProvider _buildalyzerProvider;
        private readonly IProjectMutator _projectMutator;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInputFileResolver _fileResolver;

        public ProjectOrchestrator(IBuildalyzerProvider buildalyzerProvider = null,
            IProjectMutator projectMutator = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInputFileResolver fileResolver = null)
        {
            _buildalyzerProvider = buildalyzerProvider ?? new BuildalyzerProvider();
            _projectMutator = projectMutator ?? new ProjectMutator();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
            _fileResolver = fileResolver ?? new InputFileResolver();
        }

        public IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters, ITestRunner runner = null)
        {
            if (options.IsSolutionContext)
            {
                // Analyze all projects in the solution with buildalyzer
                var solutionAnalyzerResults = AnalyzeSolution(options);
                var testProjects = solutionAnalyzerResults.Where(p => p.IsTestProject()).ToList();
                var projectsUnderTestAnalyzerResult = solutionAnalyzerResults.Where(p => !p.IsTestProject()).ToList();
                _logger.LogInformation("Found {0} source projects", projectsUnderTestAnalyzerResult.Count);
                _logger.LogInformation("Found {0} test projects", testProjects.Count);

                // Build the complete solution
                _initialBuildProcess.InitialBuild(
                    projectsUnderTestAnalyzerResult.First().TargetsFullFramework(),
                    _fileResolver.FileSystem.Path.GetDirectoryName(options.SolutionPath),
                    options.SolutionPath,
                    options.MsBuildPath);

                // create a test runner
                var projectAndTest = new SolutionTests(testProjects.Select(t => t.GetAssemblyPath()).ToList());
                runner ??= new VsTestRunnerPool(options, projectAndTest);
                

                var dependents = FindDependentProjects(projectsUnderTestAnalyzerResult);
                var projectInfos = BuildProjectInfos(options, dependents, solutionAnalyzerResults);

                // Mutate all projects in the solution
                foreach (var project in MutateSolution(options, reporters, projectsUnderTestAnalyzerResult, testProjects, solutionAnalyzerResults, dependents))
                {
                    yield return project;
                }
            }
            else
            {
                // mutate a single project from the test project context
                _logger.LogInformation("Identifying project to mutate.");
                yield return _projectMutator.MutateProject(options, reporters);
            }
        }

        private ICollection<ProjectInfo> BuildProjectInfos(StrykerOptions options,
            Dictionary<string, HashSet<string>> dependents, List<IAnalyzerResult> solutionAnalyzerResults)
        {
            var testProjects = solutionAnalyzerResults.Where(p => p.IsTestProject()).ToList();
            var projectsUnderTestAnalyzerResult = solutionAnalyzerResults.Where(p => !p.IsTestProject()).ToList();
            var result = new List<ProjectInfo>(projectsUnderTestAnalyzerResult.Count);
            foreach (var project in projectsUnderTestAnalyzerResult)
            {
                var candidateProject = dependents.ContainsKey(project.ProjectFilePath) ? dependents[project.ProjectFilePath].ToHashSet() : new HashSet<string>();

                candidateProject.Add(project.ProjectFilePath);
                var relatedTestProjects = testProjects
                    .Where(testProject => testProject.ProjectReferences.Any(reference => candidateProject.Contains(reference))).ToList();
                var projectOptions = options.Copy(
                    projectPath: project.ProjectFilePath,
                    workingDirectory: options.ProjectPath,
                    projectUnderTest: project.ProjectFilePath,
                    testProjects: relatedTestProjects.Select(x => x.ProjectFilePath));
                var info = _fileResolver.ResolveInput(projectOptions, solutionAnalyzerResults);
                result.Add(info);
            }
            return result;
        }

        private IEnumerable<IMutationTestProcess> MutateSolution(StrykerOptions options,
            IReporter reporters,
            IEnumerable<IAnalyzerResult> projectsUnderTest, 
            IEnumerable<IAnalyzerResult> testProjects,
            IEnumerable<IAnalyzerResult> solutionProjects,
            Dictionary<string, HashSet<string>> dependents)
        {

            foreach (var project in projectsUnderTest)
            {
                var projectFilePath = project.ProjectFilePath;
                var candidateProject = dependents.ContainsKey(projectFilePath) ? dependents[projectFilePath].ToHashSet() : new HashSet<string>();

                candidateProject.Add(projectFilePath);
                var relatedTestProjects = testProjects
                    .Where(testProject => testProject.ProjectReferences.Any(reference => candidateProject.Contains(reference))).ToList();

                if (relatedTestProjects.Any())
                {
                    _logger.LogDebug("Matched {0} to {1} test projects:", projectFilePath, relatedTestProjects.Count);

                    foreach (var relatedTestProjectAnalyzerResults in relatedTestProjects)
                    {
                        _logger.LogDebug("{0}", relatedTestProjectAnalyzerResults.ProjectFilePath);
                    }

                    var projectOptions = options.Copy(
                        projectPath: projectFilePath,
                        workingDirectory: options.ProjectPath,
                        projectUnderTest: projectFilePath,
                        testProjects: relatedTestProjects.Select(x => x.ProjectFilePath));

                    yield return _projectMutator.MutateProject(projectOptions, reporters, solutionProjects);
                }
                else
                {
                    _logger.LogWarning("No test projects could be found for {0}", projectFilePath);
                }
            }
        }

        private static Dictionary<string, HashSet<string>> FindDependentProjects(IEnumerable<IAnalyzerResult> projectsUnderTest)
        {
            // need to scan traverse dependencies
            // dependents contains the list of projects depending on each (non test) projects
            var dependents = new Dictionary<string, HashSet<string>>();
            foreach (var project in projectsUnderTest)
            {
                foreach (var dependent in project.ProjectReferences.ToList())
                {
                    if (!dependents.ContainsKey(dependent))
                    {
                        dependents[dependent] = new HashSet<string>();
                    }
                    // project depends on dependent
                    dependents[dependent].Add(project.ProjectFilePath);
                }
            }

            // we need to dig recursively, until no further change happens
            bool foundNewDependency;
            do
            {
                var nextDependence = new Dictionary<string, HashSet<string>>();
                foundNewDependency = false;
                foreach (var (project, dependent) in dependents)
                {
                    var newList = new HashSet<string>(dependent);
                    foreach (var sub in dependent)
                    {
                        // we need to forward the dependence to the projects which depends on projects which depend on this project
                        if (!dependents.ContainsKey(sub)) continue;
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
            _logger.LogInformation("Identifying projects to mutate in {0}. This can take a while.", options.SolutionPath);
            var manager = _buildalyzerProvider.Provide(options.SolutionPath);

            // build all projects
            var projectsAnalyzerResults = new ConcurrentBag<IAnalyzerResult>();
            _logger.LogDebug("Analysing {count} projects", manager.Projects.Count);
            try
            {
                Parallel.ForEach(manager.Projects.Values, project =>
                {
                    _logger.LogDebug("Analysing {projectFilePath}", project.ProjectFile.Path);
                    var buildResult = project.Build();
                    var projectAnalyzerResult = buildResult.Results.FirstOrDefault();
                    if (projectAnalyzerResult is { })
                    {
                        projectsAnalyzerResults.Add(projectAnalyzerResult);
                        _logger.LogDebug("Analysis of project {projectFilePath} succeeded", project.ProjectFile.Path);
                    }
                    else
                    {
                        _logger.LogWarning("Analysis of project {projectFilePath} failed", project.ProjectFile.Path);
                    }
                });
            }
            catch (AggregateException ex)
            {
                throw ex.GetBaseException();
            }

            return projectsAnalyzerResults.ToList();
        }

        private void InitializeDashboardProjectInformation(StrykerOptions options, ProjectInfo projectInfo)
        {
            var dashboardReporterEnabled = options.Reporters.Contains(Reporter.Dashboard) || options.Reporters.Contains(Reporter.All);
            var dashboardBaselineEnabled = options.WithBaseline && options.BaselineProvider == BaselineProvider.Dashboard;
            var requiresProjectInformation = dashboardReporterEnabled || dashboardBaselineEnabled;
            if (!requiresProjectInformation)
            {
                return;
            }

            // try to read the repository URL + version for the dashboard report or dashboard baseline
            var missingProjectName = string.IsNullOrEmpty(options.ProjectName);
            var missingProjectVersion = string.IsNullOrEmpty(options.ProjectVersion);
            if (missingProjectName || missingProjectVersion)
            {
                var subject = missingProjectName switch
                {
                    true when missingProjectVersion => "Project name and project version",
                    true => "Project name",
                    _ => "Project version"
                };
                var projectFilePath = projectInfo.ProjectUnderTestAnalyzerResult.ProjectFilePath;

                if (!projectInfo.ProjectUnderTestAnalyzerResult.Properties.TryGetValue("TargetPath", out var targetPath))
                {
                    throw new InputException($"Can't read {subject.ToLowerInvariant()} because the TargetPath property was not found in {projectFilePath}");
                }

                _logger.LogTrace("{Subject} missing for the dashboard reporter, reading it from {TargetPath}. " +
                                 "Note that this requires SourceLink to be properly configured in {ProjectPath}", subject, targetPath, projectFilePath);

                try
                {
                    var targetName = Path.GetFileName(targetPath);
                    using var module = ModuleDefinition.ReadModule(targetPath);

                    var details = $"To solve this issue, either specify the {subject.ToLowerInvariant()} in the stryker configuration or configure [SourceLink](https://github.com/dotnet/sourcelink#readme) in {projectFilePath}";
                    if (missingProjectName)
                    {
                        options.ProjectName = ReadProjectName(module, details);
                        _logger.LogDebug("Using {ProjectName} as project name for the dashboard reporter. (Read from the AssemblyMetadata/RepositoryUrl assembly attribute of {TargetName})", options.ProjectName, targetName);
                    }

                    if (missingProjectVersion)
                    {
                        options.ProjectVersion = ReadProjectVersion(module, details);
                        _logger.LogDebug("Using {ProjectVersion} as project version for the dashboard reporter. (Read from the AssemblyInformationalVersion assembly attribute of {TargetName})", options.ProjectVersion, targetName);
                    }
                }
                catch (Exception e) when (e is not InputException)
                {
                    throw new InputException($"Failed to read {subject.ToLowerInvariant()} from {targetPath} because of error {e.Message}");
                }
            }
        }

        private static string ReadProjectName(ModuleDefinition module, string details)
        {
            if (module.Assembly.CustomAttributes
                    .FirstOrDefault(e => e.AttributeType.Name == "AssemblyMetadataAttribute"
                                         && e.ConstructorArguments.Count == 2
                                         && e.ConstructorArguments[0].Value.Equals("RepositoryUrl"))?.ConstructorArguments[1].Value is not string repositoryUrl)
            {
                throw new InputException($"Failed to retrieve the RepositoryUrl from the AssemblyMetadataAttribute of {module.FileName}", details);
            }

            const string SchemeSeparator = "://";
            var indexOfScheme = repositoryUrl.IndexOf(SchemeSeparator, StringComparison.Ordinal);
            if (indexOfScheme < 0)
            {
                throw new InputException($"Failed to compute the project name from the repository URL ({repositoryUrl}) because it doesn't contain a scheme ({SchemeSeparator})", details);
            }

            return repositoryUrl[(indexOfScheme + SchemeSeparator.Length)..];
        }

        private static string ReadProjectVersion(ModuleDefinition module, string details)
        {
            if (module.Assembly.CustomAttributes
                    .FirstOrDefault(e => e.AttributeType.Name == "AssemblyInformationalVersionAttribute"
                                         && e.ConstructorArguments.Count == 1)?.ConstructorArguments[0].Value is not string assemblyInformationalVersion)
            {
                throw new InputException($"Failed to retrieve the AssemblyInformationalVersionAttribute of {module.FileName}", details);
            }

            return assemblyInformationalVersion;
        }
       private sealed class SolutionTests: IProjectAndTest
        {
            private readonly IReadOnlyList<string> _testAssemblies;

            public SolutionTests(IReadOnlyList<string> testAssemblies) => _testAssemblies = testAssemblies;

            public bool IsFullFramework { get; }
            public string HelperNamespace { get; }

            public IReadOnlyList<string> GetTestAssemblies() => _testAssemblies;
        }
    }
}
