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
        private IInitialisationProcess _initializationProcess;
        private readonly ILogger _logger;
        private readonly IBuildalyzerProvider _buildalyzerProvider;
        private readonly IProjectMutator _projectMutator;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInputFileResolver _fileResolver;
        private ITestRunner _runner;

        public ProjectOrchestrator(IBuildalyzerProvider buildalyzerProvider = null,
            IProjectMutator projectMutator = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInputFileResolver fileResolver = null,
            IInitialisationProcess initializationProcess = null)
        {
            _buildalyzerProvider = buildalyzerProvider ?? new BuildalyzerProvider();
            _projectMutator = projectMutator ?? new ProjectMutator();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
            _fileResolver = fileResolver ?? new InputFileResolver();
            _initializationProcess = initializationProcess;
        }

        public IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters,
            ITestRunner mockRunnerObject = null)
        {
            if (options.IsSolutionContext)
            {
                _initializationProcess ??= new InitialisationProcess(_fileResolver, _initialBuildProcess,buildalyzerProvider: _buildalyzerProvider);
                var projectInfos = _initializationProcess.GetMutableProjectsInfo(options);

                // create a test runner
                _runner = mockRunnerObject ?? new VsTestRunnerPool(options,
                    projectInfos.SelectMany(p =>p.GetTestAssemblies()).ToList(),
                    fileSystem: _fileResolver.FileSystem);

                var mutationInputs = new List<MutationTestInput>();
                var assemblyResolver = new AssemblyReferenceResolver();
                foreach (var projectInfo in projectInfos)
                {
                    var input = new MutationTestInput
                    {
                        ProjectInfo = projectInfo,
                        AssemblyReferences = assemblyResolver.LoadProjectReferences(projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                        TestRunner = _runner,
                        InitialTestRun = _initializationProcess.InitialTest(options, projectInfo, _runner)
                    };
                    mutationInputs.Add(input);
                }

                foreach (var mutationTestInput in mutationInputs)
                {
                    yield return _projectMutator.MutateProject(options, mutationTestInput, reporters);
                }
            }
            else
            {
                var initializationProcess = new InitialisationProcess(testRunner: _runner);
                var projects = initializationProcess.Initialize(options, null);
                foreach (var input in projects)
                {
                    input.InitialTestRun = initializationProcess.InitialTest(options, input.ProjectInfo, null);
                    // mutate a single project from the test project context
                    _logger.LogInformation("Identifying project to mutate.");
                    yield return _projectMutator.MutateProject(options, input, reporters);
                }
            }
        }

        /// <summary>
        /// Analyze a solution to identify projects and their related test projects
        /// </summary>
        /// <param name="options">Stryker options</param>
        /// <returns>A collection of project info</returns>
        private ICollection<ProjectInfo> ExtractProjectInfoFromSolution(StrykerOptions options)
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

            var dependents = FindDependentProjects(projectsUnderTestAnalyzerResult);
            var projectInfos = BuildProjectInfos(options, dependents, solutionAnalyzerResults);
            return projectInfos;
        }

        private ICollection<ProjectInfo> BuildProjectInfos(StrykerOptions options,
            IReadOnlyDictionary<string, HashSet<string>> dependents, List<IAnalyzerResult> solutionAnalyzerResults)
        {
            var testProjects = solutionAnalyzerResults.Where(p => p.IsTestProject()).ToList();
            var projectsUnderTestAnalyzerResult = solutionAnalyzerResults.Where(p => !p.IsTestProject()).ToList();
            var result = new List<ProjectInfo>(projectsUnderTestAnalyzerResult.Count);
            foreach (var project in projectsUnderTestAnalyzerResult.Select(p =>p.ProjectFilePath))
            {
                var relatedTestProjects = testProjects
                    .Where(testProject => testProject.ProjectReferences.Any(reference => dependents[project].Contains(reference))).Select(p =>p.ProjectFilePath).ToList();
                if (relatedTestProjects.Count > 0)
                {
                    _logger.LogDebug("Matched {0} to {1} test projects:", project, relatedTestProjects.Count);

                    foreach (var relatedTestProjectAnalyzerResults in relatedTestProjects)
                    {
                        _logger.LogDebug("{0}", relatedTestProjectAnalyzerResults);
                    }

                    var projectOptions = options.Copy(
                        projectPath: project,
                        workingDirectory: options.ProjectPath,
                        projectUnderTest: project,
                        testProjects: relatedTestProjects);
                    result.Add(_fileResolver.ResolveInput(projectOptions, solutionAnalyzerResults));
                }
                else
                {
                    _logger.LogWarning("No test projects could be found for {0}", project);
                }
            }
            return result;
        }

        private static Dictionary<string, HashSet<string>> FindDependentProjects(IEnumerable<IAnalyzerResult> projectsUnderTest)
        {
            // need to scan traverse dependencies
            // dependents contains the list of projects depending on each (non test) projects
            var dependents = projectsUnderTest.ToDictionary(p=>p.ProjectFilePath, p => new HashSet<string>(new []{p.ProjectFilePath}));

            // we need to dig recursively, until no further change happens
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
            _logger.LogInformation("Identifying projects to mutate in {0}. This can take a while.", options.SolutionPath);
            var manager = _buildalyzerProvider.Provide(options.SolutionPath);

            // build all projects
            var projectsAnalyzerResults = new ConcurrentBag<IAnalyzerResult>();
            _logger.LogDebug("Analyzing {count} projects", manager.Projects.Count);
            try
            {
                Parallel.ForEach(manager.Projects.Values, project =>
                {
                    _logger.LogDebug("Analyzing {projectFilePath}", project.ProjectFile.Path);
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
    }
}
