using System;
using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.Initialisation
{
    // For mocking purposes

    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects);
        InitialTestRun InitialTest(StrykerOptions options, ProjectInfo projectInfo);
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

        public InitialTestRun InitialTest(StrykerOptions options, ProjectInfo projectInfo)
        {
            // initial test
            var result = _initialTestProcess.InitialTest(options, projectInfo, _testRunner);

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

            if (_testRunner.DiscoverTests(projectInfo).Count != 0)
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
                    if (testProject.References.Any(r => r.Contains(framework)) && !testProject.References.Any(r => r.Contains(adapter)))
                    {
                        var message = $"Project '{testProject.ProjectFilePath}' did not report any test. This may be because it is missing an appropriate VstTest adapter for '{framework}'. " +
                                      $"Adding '{adapter}' to this project references may resolve the issue.";
                        projectInfo.ProjectWarnings.Add(message);
                        _logger.LogWarning(message);
                    }
                }
            }
        }
    }

    public class SolutionInitializationProcess : InitialisationProcessBase, IInitialisationProcess
    {
        private readonly IAssemblyReferenceResolver _referenceResolver;

        public SolutionInitializationProcess(ITestRunner testRunner, IAssemblyReferenceResolver referenceResolver = null, 
            IInitialTestProcess initialTestProcess = null)
        {
            _referenceResolver = referenceResolver ?? new AssemblyReferenceResolver();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _testRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<SolutionInitializationProcess>();
        }

        public ProjectInfo ProjectInfo { get; set; }
        public MutationTestInput Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects) =>
            new()
            {
                ProjectInfo = ProjectInfo,
                AssemblyReferences = _referenceResolver
                    .LoadProjectReferences(ProjectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner

            };
       
    }

    public class InitialisationProcess : InitialisationProcessBase, IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;

        public InitialisationProcess(
            IInputFileResolver inputFileResolver = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInitialTestProcess initialTestProcess = null,
            ITestRunner testRunner = null,
            IAssemblyReferenceResolver assemblyReferenceResolver = null)
        {
            _inputFileResolver = inputFileResolver ?? new InputFileResolver();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _testRunner = testRunner;
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
        }


        public MutationTestInput Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options, solutionProjects);

            // initial build
            if (!options.IsSolutionContext)
            {
                var testProjects = projectInfo.TestProjectAnalyzerResults.ToList();
                for (var i = 0; i < testProjects.Count; i++)
                {
                    _logger.LogInformation(
                        "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                        testProjects[i].ProjectFilePath, i + 1,
                        projectInfo.TestProjectAnalyzerResults.Count());

                    _initialBuildProcess.InitialBuild(
                        testProjects[i].TargetsFullFramework(),
                        testProjects[i].ProjectFilePath,
                        options.SolutionPath,
                        options.MsBuildPath);
                }
            }
            

            _testRunner ??= new VsTestRunnerPool(options, projectInfo);

            return new MutationTestInput
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner,
            };
        }
    }
}
