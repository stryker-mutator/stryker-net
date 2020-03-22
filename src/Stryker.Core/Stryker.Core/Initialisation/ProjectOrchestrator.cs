using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IProjectOrchestrator
    {
        IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters);
    }

    public class ProjectOrchestrator : IProjectOrchestrator
    {
        private readonly ILogger _logger;
        private readonly IInitialisationProcess _initialisationProcess;

        public ProjectOrchestrator(IInitialisationProcess initialisationProcess = null)
        {
            _initialisationProcess = initialisationProcess ?? new InitialisationProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
        }

        public IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters)
        {
            if (options.SolutionPath != null)
            {
                _logger.LogInformation("Identifying projects to mutate.");
                var manager = new AnalyzerManager(options.SolutionPath);

                var projects = manager.Projects.Where(x => !x.Value.ProjectFile.PackageReferences.Any(y => y.Name.ToLower().Contains("test"))).Select(x => x.Value).ToList();
                _logger.LogDebug("Found {0} projects", projects.Count);

                var testProjects = manager.Projects.Select(x => x.Value).Except(projects).Select(x => x.Build().Results.FirstOrDefault()).ToList();
                _logger.LogDebug("Found {0} test projects", testProjects.Count);

                foreach (var project in projects)
                {
                    var relatedTestProjects = testProjects.Where(x => x.ProjectReferences.Any(y => y == project.ProjectFile.Path)).ToList();
                    if (relatedTestProjects.Any())
                    {
                        _logger.LogDebug("Matched {0} to {1} test projects:", project.ProjectFile.Path, relatedTestProjects.Count);
                        foreach (var relatedTestProject in relatedTestProjects)
                        {
                            _logger.LogDebug("Matched {0} to {1} test projects", relatedTestProject.ProjectFilePath);
                        }
                        var projectOptions = options.ToProjectOptions(project.ProjectFile.Path,
                            project.ProjectFile.Path,
                            relatedTestProjects.Select(x => x.ProjectFilePath));
                        yield return PrepareProject(projectOptions, reporters);
                    }
                }
            }
            else
            {
                _logger.LogInformation("Identifying project to mutate.");
                yield return PrepareProject(options.ToProjectOptions(options.BasePath, null, null), reporters);
            }
        }

        private IMutationTestProcess PrepareProject(StrykerProjectOptions options, IReporter reporters)
        {
            // initialize
            var input = _initialisationProcess.Initialize(options);

            var process = new MutationTestProcess(
                mutationTestInput: input,
                reporter: reporters,
                mutationTestExecutor: new MutationTestExecutor(input.TestRunner),
                options: options);

            // initial test
            input.TimeoutMs = _initialisationProcess.InitialTest(options);

            // mutate
            process.Mutate();
            return process;
        }
    }
}
