using System;
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
        private readonly IInitialisationProcessProvider _initialisationProcessProvider;
        private readonly IMutationTestProcessProvider _mutationTestProcessProvider;
        private readonly IBuildalyzerProvider _buildalyzerProvider;

        public ProjectOrchestrator(IInitialisationProcessProvider initialisationProcessProvider = null,
            IMutationTestProcessProvider mutationTestProcessProvider = null,
            IBuildalyzerProvider buildalyzerProvider = null)
        {
            _initialisationProcessProvider = initialisationProcessProvider ?? new InitialisationProcessProvider();
            _mutationTestProcessProvider = mutationTestProcessProvider ?? new MutationTestProcessProvider();
            _buildalyzerProvider = buildalyzerProvider ?? new BuildalyzerProvider();
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
                yield return MutateProject(options.Copy(options.BasePath, null, null), reporters);
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

                    yield return MutateProject(projectOptions, reporters);
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
            var manager = _buildalyzerProvider.Provide(options.SolutionPath);

            // build all projects
            var projectsAnalyzerResults = new List<IAnalyzerResult>();
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

            return projectsAnalyzerResults;
        }

        private static bool IsSolutionContext(StrykerOptions options) =>
            options.SolutionPath != null && options.BasePath == Path.GetDirectoryName(options.SolutionPath);

        private IMutationTestProcess MutateProject(IStrykerOptions options, IReporter reporters)
        {
            // get a new instance of InitialisationProcess for each project
            var initialisationProcess = _initialisationProcessProvider.Provide();
            // initialize
            var input = initialisationProcess.Initialize(options);

            var process = _mutationTestProcessProvider.Provide(
                mutationTestInput: input,
                reporter: reporters,
                mutationTestExecutor: new MutationTestExecutor(input.TestRunner),
                options: options);

            // initial test
            input.TimeoutMs = initialisationProcess.InitialTest(options);

            // mutate
            process.Mutate();

            return process;
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
