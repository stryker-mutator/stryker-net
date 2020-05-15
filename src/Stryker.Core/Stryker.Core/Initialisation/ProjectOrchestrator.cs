using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            if (options.SolutionPath != null && Path.GetFullPath(options.BasePath) == Path.GetDirectoryName(options.SolutionPath))
            {
                // mutate all projects in the solution
                _logger.LogInformation("Identifying projects to mutate.");
                var manager = _buildalyzerProvider.Provide(options.SolutionPath);

                // build all projects
                var projectsAnalyzerResults = new List<IAnalyzerResult>();
                _logger.LogDebug("Analysing {count} projects", manager.Projects.Count);

                Parallel.ForEach(manager.Projects, project =>
                {
                    _logger.LogDebug("Analysing {projectFilePath}", project.Value.ProjectFile.Path);
                    var projectAnalyzerResult = project.Value.Build().Results.FirstOrDefault();
                    if (projectAnalyzerResult is { })
                    {
                        projectsAnalyzerResults.Add(projectAnalyzerResult);
                        _logger.LogDebug("Analysis of project {projectFilePath} succeeded", project.Value.ProjectFile.Path);
                    }
                    else
                    {
                        _logger.LogWarning("Analysis of project {projectFilePath} failed", project.Value.ProjectFile.Path);
                    }
                });

                var projectsUnderTestAnalyzerResults = new List<IAnalyzerResult>();
                foreach (var project in projectsAnalyzerResults)
                {
                    if (project.Properties.ContainsKey("IsTestProject"))
                    {
                        if (project.Properties["IsTestProject"].Equals(
                            "False",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            projectsUnderTestAnalyzerResults.Add(project);
                        }
                    }
                    else if (project.Properties.ContainsKey("ProjectTypeGuids"))
                    {
                        if (!project.Properties["ProjectTypeGuids"]
                            .Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}"))
                        {
                            projectsUnderTestAnalyzerResults.Add(project);
                        }
                    }
                    else
                    {
                        projectsUnderTestAnalyzerResults.Add(project);
                    }
                }

                var testProjectsAnalyzerResults = projectsAnalyzerResults.Except(projectsUnderTestAnalyzerResults);

                _logger.LogDebug("Found {0} projects under test", projectsUnderTestAnalyzerResults.Count());
                _logger.LogDebug("Found {0} test projects", testProjectsAnalyzerResults.Count());

                foreach (var project in projectsUnderTestAnalyzerResults)
                {
                    var projectFilePath = project.ProjectFilePath;
                    var relatedTestProjects = testProjectsAnalyzerResults.Where(x => x.ProjectReferences.Any(y => y == projectFilePath));
                    if (relatedTestProjects.Any())
                    {
                        _logger.LogDebug("Matched {0} to {1} test projects:", projectFilePath, relatedTestProjects.Count());
                        foreach (var relatedTestProjectAnalyzerResults in relatedTestProjects)
                        {
                            _logger.LogDebug("{0}", relatedTestProjectAnalyzerResults.ProjectFilePath);
                        }
                        var projectOptions = options.ToProjectOptions(
                            basePath: projectFilePath,
                            projectUnderTest: projectFilePath,
                            testProjects: relatedTestProjects.Select(x => x.ProjectFilePath));
                        yield return PrepareProject(projectOptions, reporters);
                    }
                }
            }
            else
            {
                // mutate a single project
                _logger.LogInformation("Identifying project to mutate.");
                yield return PrepareProject(options.ToProjectOptions(options.BasePath, null, null), reporters);
            }
        }

        private IMutationTestProcess PrepareProject(StrykerProjectOptions options, IReporter reporters)
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
    }
}
