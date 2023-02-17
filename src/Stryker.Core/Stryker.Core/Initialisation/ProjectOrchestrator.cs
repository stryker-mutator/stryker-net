using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Microsoft.Extensions.Logging;
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

        public ProjectOrchestrator(IBuildalyzerProvider buildalyzerProvider = null,
            IProjectMutator projectMutator = null,
            IInitialBuildProcess initialBuildProcess = null)
        {
            _buildalyzerProvider = buildalyzerProvider ?? new BuildalyzerProvider();
            _projectMutator = projectMutator ?? new ProjectMutator();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
        }

        public IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters, ITestRunner runner = null)
        {
            if (options.IsSolutionContext)
            {
                // Analyze all projects in the solution with buildalyzer
                var solutionAnalyzerResults = AnalyzeSolution(options);

                var projectsUnderTestAnalyzerResult = FindProjectsUnderTest(solutionAnalyzerResults).ToList();

                var testProjects = solutionAnalyzerResults.Except(projectsUnderTestAnalyzerResult).ToList();

                _logger.LogInformation("Found {0} source projects", projectsUnderTestAnalyzerResult.Count);
                _logger.LogInformation("Found {0} test projects", testProjects.Count);
                // Build the complete solution
                _initialBuildProcess.InitialBuild(
                    projectsUnderTestAnalyzerResult.First().TargetsFullFramework(),
                    Path.GetDirectoryName(options.SolutionPath),
                    options.SolutionPath,
                    options.MsBuildPath);

                // create a test runner
                var projectAndTest = new SolutionTests(testProjects.Select(t => t.GetAssemblyPath()).ToList());
                runner ??= new VsTestRunnerPool(options, projectAndTest);
                var dependents = FindDependentProjects(projectsUnderTestAnalyzerResult);
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

        private IEnumerable<IAnalyzerResult> FindProjectsUnderTest(IEnumerable<IAnalyzerResult> projectsAnalyzerResults)
        {
            foreach (var project in projectsAnalyzerResults)
            {
                if (!project.IsTestProject())
                {
                    yield return project;
                }
            }
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
