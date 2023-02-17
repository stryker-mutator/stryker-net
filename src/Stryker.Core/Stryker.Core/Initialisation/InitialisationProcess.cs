using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.Initialisation
{
    // For mocking purposes

    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects);
        InitialTestRun InitialTest(StrykerOptions options);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private ITestRunner _testRunner;
        private ProjectInfo _projectInfo;
        private readonly ILogger _logger;


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
            _projectInfo = _inputFileResolver.ResolveInput(options, solutionProjects);

            // initial build
            if (!options.IsSolutionContext)
            {
                var testProjects = _projectInfo.TestProjectAnalyzerResults.ToList();
                for (var i = 0; i < testProjects.Count; i++)
                {
                    _logger.LogInformation(
                        "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                        testProjects[i].ProjectFilePath, i + 1,
                        _projectInfo.TestProjectAnalyzerResults.Count());

                    _initialBuildProcess.InitialBuild(
                        testProjects[i].TargetsFullFramework(),
                        testProjects[i].ProjectFilePath,
                        options.SolutionPath,
                        options.MsBuildPath);
                }
            }
            

            _testRunner ??= new VsTestRunnerPool(options, _projectInfo);

            var input = new MutationTestInput
            {
                ProjectInfo = _projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(_projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner,
            };

            return input;
        }

        public InitialTestRun InitialTest(StrykerOptions options)
        {
            // initial test
            var result = _initialTestProcess.InitialTest(options, _projectInfo, _testRunner);

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

            if (_testRunner.DiscoverTests(_projectInfo).Count != 0)
            {
                return result;
            }
            // no test have been discovered, diagnose this
            DiagnoseLackOfDetectedTest(_projectInfo);
            if (options.IsSolutionContext)
            {
                return result;
            }
            throw new InputException("No test has been detected. Make sure your test project contains test and is compatible with VsTest."+string.Join(Environment.NewLine, _projectInfo.ProjectWarnings));
        }

        private static readonly Dictionary<string, string> TestFrameworks = new()
        {
            ["xunit.core"] = "xunit.runner.visualstudio",
            ["nunit.framework"] = "NUnit3.TestAdapter",
            ["Microsoft.VisualStudio.TestPlatform.TestFramework"] = "Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter"
        };

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
}
