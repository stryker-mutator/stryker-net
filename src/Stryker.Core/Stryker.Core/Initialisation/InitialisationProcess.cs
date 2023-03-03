using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.Initialisation
{
    // For mocking purposes

    public interface IInitialisationProcess
    {
        IEnumerable<MutationTestInput> Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects);
        InitialTestRun InitialTest(StrykerOptions options, ProjectInfo projectInfo, ITestRunner testRunner = null);
        IEnumerable<ProjectInfo> GetMutableProjectsInfo(StrykerOptions options);
    }


    public class InitialisationProcessBase
    {
        protected IInitialTestProcess _initialTestProcess;
        protected ITestRunner _testRunner;
        protected ILogger _logger;

        private static readonly Dictionary<string, string> TestFrameworks = new()
        {
            ["xunit.core"] = "xunit.runner.visualstudio",
            ["nunit.framework"] = "NUnit3.TestAdapter",
            ["Microsoft.VisualStudio.TestPlatform.TestFramework"] = "Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter"
        };

        public InitialTestRun InitialTest(StrykerOptions options, ProjectInfo projectInfo, ITestRunner testRunner = null)
        {
            testRunner ??= _testRunner;
            // initial test
            var result = _initialTestProcess.InitialTest(options, projectInfo, testRunner);

            if (!result.Result.FailingTests.IsEmpty)
            {
                var failingTestsCount = result.Result.FailingTests.Count;
                if (options.BreakOnInitialTestFailure)
                {
                    throw new InputException("Initial testrun has failing tests.", result.Result.ResultMessage);
                }

                if (!options.IsSolutionContext && ((double)failingTestsCount) / result.Result.RanTests.Count >= .5)
                {
                    throw new InputException("Initial testrun has more than 50% failing tests.", result.Result.ResultMessage);
                }
                
                _logger.LogWarning($"{(failingTestsCount == 1 ? "A test is": $"{failingTestsCount} tests are")} failing. Stryker will continue but outcome will be impacted.");
            }

            if (testRunner.DiscoverTests(projectInfo).Count != 0)
            {
                return result;
            }
            // no test have been discovered, diagnose this
            DiagnoseLackOfDetectedTest(projectInfo);
            if (options.IsSolutionContext)
            {
                return result;
            }
            throw new InputException("No test has been detected. Make sure your test project contains test and is compatible with VsTest."+string.Join(Environment.NewLine, projectInfo.ProjectWarnings));
        }

        public void DiagnoseLackOfDetectedTest(ProjectInfo projectInfo)
        {
            foreach (var testProject in projectInfo.TestProjectAnalyzerResults)
            {
                foreach (var (framework, adapter) in TestFrameworks)
                {
                    if (!testProject.References.Any(r => r.Contains(framework)) ||
                        testProject.References.Any(r => r.Contains(adapter))) continue;
                    var message = $"Project '{testProject.ProjectFilePath}' did not report any test. This may be because it is missing an appropriate VstTest adapter for '{framework}'. " +
                                  $"Adding '{adapter}' to this project references may resolve the issue.";
                    projectInfo.ProjectWarnings.Add(message);
                    _logger.LogWarning(message);
                }
            }
        }
    }

    public class InitialisationProcess : InitialisationProcessBase, IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private readonly IBuildalyzerProvider _buildalyzerProvider;

        public InitialisationProcess(
            IInputFileResolver inputFileResolver = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInitialTestProcess initialTestProcess = null,
            ITestRunner testRunner = null,
            IAssemblyReferenceResolver assemblyReferenceResolver = null,
            IBuildalyzerProvider buildalyzerProvider = null)
        {
            _buildalyzerProvider = buildalyzerProvider ?? new BuildalyzerProvider();
            _inputFileResolver = inputFileResolver ?? new InputFileResolver();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _testRunner = testRunner;
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
        }


        public IEnumerable<ProjectInfo> GetMutableProjectsInfo(StrykerOptions options) => options.IsSolutionContext ? ExtractProjectInfoFromSolution(options) : new[]{ _inputFileResolver.ResolveInput(options, null)};

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
                _inputFileResolver.FileSystem.Path.GetDirectoryName(options.SolutionPath),
                options.SolutionPath,
                options.MsBuildPath);

            var dependents = FindDependentProjects(projectsUnderTestAnalyzerResult);
            var projectInfos = BuildProjectInfos(options, dependents, solutionAnalyzerResults);
            return projectInfos;
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
                    result.Add(_inputFileResolver.ResolveInput(projectOptions, solutionAnalyzerResults));
                }
                else
                {
                    _logger.LogWarning("No test projects could be found for {0}", project);
                }
            }
            return result;
        }

        public IEnumerable<MutationTestInput> Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects)
        {
            // resolve project info
            var projectInfo = GetProjectsInfo(options, solutionProjects).ToList();

            var testProjects = projectInfo.SelectMany(p=>p.TestProjectAnalyzerResults).ToList();
            // initial build
            if (!options.IsSolutionContext)
            {
                BuildProject(options, testProjects);
            }

            _testRunner ??= new VsTestRunnerPool(options, testProjects.Select(p =>p.GetAssemblyPath()).ToList());
            foreach (var info in projectInfo)
            {
                yield return new MutationTestInput
                {
                    ProjectInfo = info,
                    AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(info.ProjectUnderTestAnalyzerResult.References).ToList(),
                    TestRunner = _testRunner,
                };
            }
        }

        private void BuildProject(StrykerOptions options, List<IAnalyzerResult> testProjects)
        {
            for (var i = 0; i < testProjects.Count; i++)
            {
                _logger.LogInformation(
                    "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                    testProjects[i].ProjectFilePath, i + 1,
                    testProjects.Count());

                _initialBuildProcess.InitialBuild(
                    testProjects[i].TargetsFullFramework(),
                    testProjects[i].ProjectFilePath,
                    options.SolutionPath,
                    options.MsBuildPath);
            }
        }

        private IEnumerable<ProjectInfo> GetProjectsInfo(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects)
        {
            yield return _inputFileResolver.ResolveInput(options, solutionProjects);
        }
    }
}
