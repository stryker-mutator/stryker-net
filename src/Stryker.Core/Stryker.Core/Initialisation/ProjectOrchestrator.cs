using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation.SolutionAnalyzer;
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
        private readonly ISolutionAnalyzerManagerProvider _solutionAnalyzerManagerProvider;
        private readonly IProjectMutator _projectMutator;

        public ProjectOrchestrator(ISolutionAnalyzerManagerProvider solutionAnalyzerManagerProvider = null,
            IProjectMutator projectMutator = null)
        {
            _solutionAnalyzerManagerProvider = solutionAnalyzerManagerProvider ?? new SolutionAnalyzerManagerProvider();
            _projectMutator = projectMutator ?? new ProjectMutator();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
        }

        public IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters)
        {
            if (IsSolutionContext(options))
            {
                // Build and analyze all projects in the solution with buildalyzer
                var solutionProjects = AnalyzeSolution(options);

                var projectsUnderTest = FindProjectsUnderTest(solutionProjects);

                var testProjects = solutionProjects.Except(projectsUnderTest);

                _logger.LogDebug("Found {0} projects under test", projectsUnderTest.Count());
                _logger.LogDebug("Found {0} test projects", testProjects.Count());

                // Mutate all projects in the solution
                foreach (var project in MutateSolution(options, reporters, projectsUnderTest, testProjects))
                {
                    yield return project;
                }
            }
            else
            {
                // mutate a single project from the test project context
                _logger.LogInformation("Identifying project to mutate.");
                yield return _projectMutator.MutateProject(options.Copy(options.BasePath, options.ProjectUnderTestName, options.TestProjects), reporters);
            }
        }

        private IEnumerable<IMutationTestProcess> MutateSolution(StrykerOptions options, IReporter reporters, IEnumerable<IAnalyzerResult> projectsUnderTest, IEnumerable<IAnalyzerResult> testProjects)
        {
            foreach (var project in projectsUnderTest)
            {
                var projectFilePath = project.ProjectFilePath;
                var relatedTestProjects = testProjects
                    .Where(testProject => testProject.ProjectReferences.Any(reference => reference == projectFilePath));

                if (relatedTestProjects.Any())
                {
                    _logger.LogDebug("Matched {0} to {1} test projects:", projectFilePath, relatedTestProjects.Count());

                    foreach (var relatedTestProjectAnalyzerResults in relatedTestProjects)
                    {
                        _logger.LogDebug("{0}", relatedTestProjectAnalyzerResults.ProjectFilePath);
                    }

                    var projectOptions = options.Copy(
                        basePath: projectFilePath,
                        projectUnderTest: projectFilePath,
                        testProjects: relatedTestProjects.Select(x => x.ProjectFilePath));

                    yield return _projectMutator.MutateProject(projectOptions, reporters);
                }
                else
                {
                    _logger.LogWarning("No test projects could be found for {0}", projectFilePath);
                }
            }
        }

        private List<IAnalyzerResult> AnalyzeSolution(StrykerOptions options)
        {
            _logger.LogInformation("Identifying projects to mutate. This can take a while.");
            var manager = _solutionAnalyzerManagerProvider.Provide(options.SolutionPath);

            // build all projects
            var projectsAnalyzerResults = new ConcurrentBag<IAnalyzerResult>();
            _logger.LogDebug("Analysing {count} projects", manager.Projects.Count);
            try
            {
                Parallel.ForEach(manager.Projects.Values, project =>
                {
                    _logger.LogDebug("Analysing {projectFilePath}", project.ProjectFilePath);
                    var projectAnalyzerResult = project.Build();
                    if (projectAnalyzerResult is { })
                    {
                        projectsAnalyzerResults.Add(projectAnalyzerResult);
                        _logger.LogDebug("Analysis of project {projectFilePath} succeeded", project.ProjectFilePath);
                    }
                    else
                    {
                        _logger.LogWarning("Analysis of project {projectFilePath} failed", project.ProjectFilePath);
                    }
                });
            }
            catch (AggregateException ex)
            {
                throw ex.GetBaseException();
            }

            return projectsAnalyzerResults.ToList();
        }

        private static bool IsSolutionContext(StrykerOptions options) =>
            options.SolutionPath != null && FilePathUtils.NormalizePathSeparators(options.BasePath) == FilePathUtils.NormalizePathSeparators(Path.GetDirectoryName(options.SolutionPath));

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
