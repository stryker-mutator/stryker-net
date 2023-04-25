using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation
{
    // For mocking purposes

    public interface IInitialisationProcess
    {
        /// <summary>
        /// Gets all projects to mutate based on the given options
        /// </summary>
        /// <param name="options">stryker options</param>
        /// <returns>an enumeration of <see cref="ProjectInfo"/>, one for each found project (if any).</returns>
        IReadOnlyCollection<SourceProjectInfo> GetMutableProjectsInfo(StrykerOptions options);

        void BuildProjects(StrykerOptions options, IEnumerable<SourceProjectInfo> projects);

        IReadOnlyCollection<MutationTestInput> GetMutationTestInputs(StrykerOptions options,
            IReadOnlyCollection<SourceProjectInfo> projects, ITestRunner runner);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IBuildalyzerProvider _buildalyzerProvider;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly ILogger _logger;

        public InitialisationProcess(
            IInputFileResolver inputFileResolver = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInitialTestProcess initialTestProcess = null,
            IBuildalyzerProvider buildalyzerProvider = null)
        {
            _buildalyzerProvider = buildalyzerProvider ?? new BuildalyzerProvider();
            _inputFileResolver = inputFileResolver ?? new InputFileResolver();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<SourceProjectInfo> GetMutableProjectsInfo(StrykerOptions options)
        {
            _logger.LogInformation("Analysis starting.");
            try
            {
                if (!options.IsSolutionContext)
                {
                    var testProjectsResult = _inputFileResolver.ResolveTestProjects(options, null);
                    // project mode
                    return _inputFileResolver.ResolveSourceProjectInfos(options, testProjectsResult,null);
                }

                // Analyze all projects in the solution with buildalyzer
                var solutionAnalyzerResults = AnalyzeSolution(options);
                var testProjects = solutionAnalyzerResults.Where(p => p.IsTestProject()).ToList();
                var projectsUnderTestAnalyzerResult = solutionAnalyzerResults.Where(p => !p.IsTestProject()).ToList();

                _logger.LogInformation("Found {0} source projects", projectsUnderTestAnalyzerResult.Count);
                _logger.LogInformation("Found {0} test projects", testProjects.Count);

                var dependents = FindDependentProjects(projectsUnderTestAnalyzerResult);
                return BuildProjectInfos(options, dependents, projectsUnderTestAnalyzerResult, testProjects, solutionAnalyzerResults);
            }
            finally
            {
                _logger.LogInformation("Analysis complete.");
            }
        }


        /// <inheritdoc/>
        public void BuildProjects(StrykerOptions options, IEnumerable<SourceProjectInfo> projects)
        {
            if (options.IsSolutionContext)
            {
                var framework = projects.Any(p => p.IsFullFramework);
                // Build the complete solution
                _logger.LogInformation("Building solution {0}", Path.GetRelativePath(options.WorkingDirectory ,options.SolutionPath));
                _initialBuildProcess.InitialBuild(
                    framework,
                    _inputFileResolver.FileSystem.Path.GetDirectoryName(options.SolutionPath),
                    options.SolutionPath,
                    options.MsBuildPath);
            }
            else
            {
                // build every test projects
                var testProjects = projects.SelectMany(p => p.TestProjectsInfo.AnalyzerResults).ToList();
                for (var i = 0; i < testProjects.Count; i++)
                {
                    _logger.LogInformation(
                        "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                        testProjects[i].ProjectFilePath, i + 1,
                        testProjects.Count);

                    _initialBuildProcess.InitialBuild(
                        testProjects[i].TargetsFullFramework(),
                        testProjects[i].ProjectFilePath,
                        options.SolutionPath,
                        options.MsBuildPath);
                }
            }
        }

        private static Dictionary<string, HashSet<string>> FindDependentProjects(IList<IAnalyzerResult> projectsUnderTest)
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
            var manager = _buildalyzerProvider.Provide(options.SolutionPath);

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
                    if (projectAnalyzerResult is { })
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
            IReadOnlyCollection<IAnalyzerResult> testProjects,
            IReadOnlyCollection<IAnalyzerResult> solutionAnalyzerResults)
        {
            var result = new List<SourceProjectInfo>(projectsUnderTestAnalyzerResult.Count);
            foreach (var project in projectsUnderTestAnalyzerResult.Select(p =>p.ProjectFilePath))
            {
                var projectLogName = Path.GetRelativePath(Path.GetDirectoryName(options.SolutionPath), project);
                var analyzerResultsForTestProjects = testProjects
                    .Where(testProject => testProject.ProjectReferences.Any(reference => dependents[project].Contains(reference)));
                var relatedTestProjects = analyzerResultsForTestProjects.Select(p =>p.ProjectFilePath).ToList();
                if (relatedTestProjects.Count > 0)
                {
                    _logger.LogDebug("Matched {0} to {1} test projects:", projectLogName, relatedTestProjects.Count);

                    foreach (var relatedTestProjectAnalyzerResults in relatedTestProjects)
                    {
                        _logger.LogDebug("{0}", relatedTestProjectAnalyzerResults);
                    }

                    var projectOptions = options.Copy(
                        project,
                        options.ProjectPath,
                         project,
                        testProjects: relatedTestProjects);
                    var resolveSourceProjectInfo = _inputFileResolver.ResolveSourceProjectInfos(projectOptions, analyzerResultsForTestProjects, solutionAnalyzerResults);

                    resolveSourceProjectInfo.Single().TestProjectsInfo =
                        new TestProjectsInfo(_inputFileResolver.FileSystem)
                        {
                            TestProjects = analyzerResultsForTestProjects.Select(testProjectAnalyzerResult => new TestProject(_inputFileResolver.FileSystem, testProjectAnalyzerResult)).ToList()
                        };
                    result.Add(resolveSourceProjectInfo.Single());
                }
                else
                {
                    _logger.LogWarning("Project {0} will not be mutated because Stryker did not find a test project for it.", projectLogName);
                }
            }
            return result;
        }

        public IReadOnlyCollection<MutationTestInput> GetMutationTestInputs(StrykerOptions options, IReadOnlyCollection<SourceProjectInfo> projects, ITestRunner runner)
        {
            var result = new List<MutationTestInput>();
            foreach (var info in projects)
            {
                result.Add(new MutationTestInput
                {
                    SourceProjectInfo = info,
                    TestProjectsInfo = info.TestProjectsInfo,
                    TestRunner = runner,
                    InitialTestRun = InitialTest(options, info, runner)
                });
            }

            return result;
        }

        private InitialTestRun InitialTest(StrykerOptions options, SourceProjectInfo projectInfo, ITestRunner testRunner)
        {
            DiscoverTests(projectInfo, testRunner);

            // initial test
            _logger.LogInformation("Number of tests found: {0} for project {1}. Initial test run started.",
                testRunner.GetTests(projectInfo).Count,
                projectInfo.AnalyzerResult.ProjectFilePath);

            var result = _initialTestProcess.InitialTest(options, projectInfo, testRunner);


            if (!result.Result.FailingTests.IsEmpty)
            {
                var failingTestsCount = result.Result.FailingTests.Count;
                if (options.BreakOnInitialTestFailure)
                {
                    throw new InputException("Initial testrun has failing tests.", result.Result.ResultMessage);
                }

                if (!options.IsSolutionContext && (double)failingTestsCount / result.Result.ExecutedTests.Count >= .5)
                {
                    throw new InputException("Initial testrun has more than 50% failing tests.", result.Result.ResultMessage);
                }
                
                _logger.LogWarning($"{(failingTestsCount == 1 ? "A test is": $"{failingTestsCount} tests are")} failing. Stryker will continue but outcome will be impacted.");
            }

            if (!result.Result.ExecutedTests.IsEmpty)
            {
                return result;
            }
            // no test have been discovered, diagnose this
            if (options.IsSolutionContext)
            {
                return result;
            }
            throw new InputException("No test has been detected. Make sure your test project contains test and is compatible with VsTest."+string.Join(Environment.NewLine, projectInfo.Warnings));
        }

        private static readonly Dictionary<string, (string assembly, string package)> TestFrameworks = new()
        {
            ["xunit.core"] = ("xunit.runner.visualstudio","xunit.runner.visualstudio"),
            ["nunit.framework"] = ("NUnit3.TestAdapter", "NUnit3TestAdapter"),
            ["Microsoft.VisualStudio.TestPlatform.TestFramework"] = ("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter","MSTest.TestAdapter")
        };

        private void DiscoverTests(SourceProjectInfo projectInfo, ITestRunner testRunner)
        {
            foreach (var testProject in projectInfo.TestProjectsInfo.AnalyzerResults)
            {
                if (testRunner.DiscoverTests(testProject.GetAssemblyPath()))
                {
                    continue;
                }

                var causeFound = false;
                foreach (var (framework, (adapter, package)) in
                         TestFrameworks.Where(t => testProject.References.Any(r => r.Contains(t.Key))))
                {

                    if (testProject.References.Any(r => r.Contains(adapter)))
                    {
                        continue;
                    }

                    causeFound = true;
                    var message =
                        $"Project '{testProject.ProjectFilePath}' did not report any test.";
                    if (testProject.PackageReferences?.ContainsKey(package) == true)
                    {
                        message+=$" This may be because the test adapter package, {package}, failed to deploy. " +
                                 "Check if any dependency is missing or there is a version conflict.";
                    }
                    else
                    {
                        message+=$" This may be because it is missing an appropriate VsTest adapter for '{framework}'. " +
                                 $"Adding '{adapter}' to this project references may resolve the issue.";
                    }
                    projectInfo.LogError(message);
                    _logger.LogWarning(message);
                }

                if (!causeFound)
                {
                    var message = $"No test detected for project '{testProject.ProjectFilePath}'. No cause identified.";
                    projectInfo.LogError(message);
                    _logger.LogWarning(message);
                }
            }
        }
    }
}
