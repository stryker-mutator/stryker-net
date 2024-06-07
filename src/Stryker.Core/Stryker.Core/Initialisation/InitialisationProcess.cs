using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation;
// For mocking purposes

public interface IInitialisationProcess
{
    /// <summary>
    /// Gets all projects to mutate based on the given options
    /// </summary>
    /// <param name="options">stryker options</param>
    /// <returns>an enumeration of <see cref="SourceProjectInfo"/>, one for each found project (if any).</returns>
    IReadOnlyCollection<SourceProjectInfo> GetMutableProjectsInfo(StrykerOptions options);

    void BuildProjects(StrykerOptions options, IEnumerable<SourceProjectInfo> projects);

    IReadOnlyCollection<MutationTestInput> GetMutationTestInputs(StrykerOptions options,
        IReadOnlyCollection<SourceProjectInfo> projects, ITestRunner runner);
}

public class InitialisationProcess : IInitialisationProcess
{
    private readonly IInputFileResolver _inputFileResolver;
    private readonly IInitialBuildProcess _initialBuildProcess;
    private readonly IInitialTestProcess _initialTestProcess;
    private readonly ILogger _logger;

    public InitialisationProcess(
        IInputFileResolver inputFileResolver,
        IInitialBuildProcess initialBuildProcess = null,
        IInitialTestProcess initialTestProcess = null)
    {
        _inputFileResolver = inputFileResolver;
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
            // project mode
            return _inputFileResolver.ResolveSourceProjectInfos(options);
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
                _logger.LogInformation("Building solution {0}",
                    Path.GetRelativePath(options.WorkingDirectory, options.SolutionPath));
            _initialBuildProcess.InitialBuild(
                framework,
                _inputFileResolver.FileSystem.Path.GetDirectoryName(options.SolutionPath),
                options.SolutionPath, options.Configuration, options.MsBuildPath);
        }
        else
        {
            // build every test projects
            var testProjects = projects.SelectMany(p => p.TestProjectsInfo.AnalyzerResults).Distinct().ToList();
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
                    options.Configuration,
                    options.MsBuildPath);
            }
        }

        // perform post build update (to capture some content files in C# project for example)
        foreach (var project in projects)
        {
          project.OnProjectBuilt?.Invoke();
        }   
    }

        public IReadOnlyCollection<MutationTestInput> GetMutationTestInputs(StrykerOptions options,
            IReadOnlyCollection<SourceProjectInfo> projects, ITestRunner runner)
    {
        var result = new List<MutationTestInput>();
        foreach (var info in projects)
        {
            result.Add(new MutationTestInput
            {
                SourceProjectInfo = info,
                TestProjectsInfo = info.TestProjectsInfo,
                TestRunner = runner,
                    InitialTestRun = InitialTest(options, info, runner, projects.Count == 1)
            });
        }

        return result;
    }

    private InitialTestRun InitialTest(StrykerOptions options, SourceProjectInfo projectInfo,
        ITestRunner testRunner, bool throwIfFails)
    {
        DiscoverTests(projectInfo, testRunner);

        // initial test
            _logger.LogInformation(
                "Number of tests found: {TestCount} for project {ProjectFilePath}. Initial test run started.",
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

            if (throwIfFails && (double)failingTestsCount / result.Result.ExecutedTests.Count >= .5)
            {
                throw new InputException("Initial testrun has more than 50% failing tests.",
                        result.Result.ResultMessage);
            }

                _logger.LogWarning(
                    "{failingTestsCount} tests are failing. Stryker will continue but outcome will be impacted.",
                    failingTestsCount);
        }

        if (!result.Result.ExecutedTests.IsEmpty || !throwIfFails)
        {
            return result;
        }

            throw new InputException(
                "No test has been detected. Make sure your test project contains test and is compatible with VsTest." +
                string.Join(Environment.NewLine, projectInfo.Warnings));
    }

    private static readonly Dictionary<string, (string assembly, string package)> TestFrameworks = new()
    {
        ["xunit.core"] = ("xunit.runner.visualstudio","xunit.runner.visualstudio"),
        ["nunit.framework"] = ("NUnit3.TestAdapter", "NUnit3TestAdapter"),
        ["Microsoft.VisualStudio.TestPlatform.TestFramework"] = 
                ("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter", "MSTest.TestAdapter")
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
                    message+=$" This may be because the test adapter package, {package}, failed to deploy or run. " +
                             "Check if any dependency is missing or there is a version conflict, check the testdiscovery logs or explore with VsTest.console.";
                }
                else
                {
                        message +=
                            $" This may be because it is missing an appropriate VsTest adapter for '{framework}'. " +
                             $"Adding '{adapter}' to this project references may resolve the issue.";
                }

                projectInfo.LogError(message);
                _logger.LogWarning(message);
            }

            if (causeFound)
            {
                continue;
            }
            var messageForNoReason = $"No test detected for project '{testProject.ProjectFilePath}'. No cause identified.";
            projectInfo.LogError(messageForNoReason);
            _logger.LogWarning(messageForNoReason);
        }
    }
}
