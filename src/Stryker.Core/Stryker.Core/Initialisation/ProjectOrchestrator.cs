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

namespace Stryker.Core.Initialisation
{
    public interface IProjectOrchestrator
    {
        IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters);
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

        public IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters)
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

                // Mutate all projects in the solution
                foreach (var project in MutateSolution(options, reporters, projectsUnderTestAnalyzerResult, testProjects, solutionAnalyzerResults))
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
            IEnumerable<IAnalyzerResult> solutionProjects)
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
                    dependents[dependent].Add(project.ProjectFilePath);
                }
            }

            // we need to dig recursively, until no further change happens
            bool foundNewDependency;
            do
            {
                foundNewDependency = false;
                foreach (var project in dependents.Values)
                {
                    foreach (var dependency in project.ToList())
                    {
                        if (!dependents.ContainsKey(dependency))
                        {
                            continue;
                        }
                        foreach (var sub in dependents[dependency])
                        {
                            foundNewDependency = project.Add(sub) || foundNewDependency;
                        }
                    }
                }
            } while (foundNewDependency);

            foreach (var project in projectsUnderTest)
            {
                var projectFilePath = project.ProjectFilePath;
                HashSet<string> candidateProject;
                if (dependents.ContainsKey(projectFilePath))
                {;
                    candidateProject = dependents[projectFilePath].ToHashSet();
                }
                else
                {
                    candidateProject = new HashSet<string>();
                }

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
    }
}
