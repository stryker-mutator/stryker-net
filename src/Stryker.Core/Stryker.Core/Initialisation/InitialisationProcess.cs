using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

public interface IInitialisationProcess
{
    /// <summary>
    /// Gets all projects to mutate based on the given options
    /// </summary>
    /// <param name="options">stryker options</param>
    /// <returns>an enumeration of <see cref="SourceProjectInfo"/>, one for each found project (if any).</returns>
    RelatedSourceProjectsInfo GetMutableProjectsInfo(IStrykerOptions options);

    void BuildProjects(IStrykerOptions options, RelatedSourceProjectsInfo projects);

    Task<IReadOnlyCollection<MutationTestInput>> GetMutationTestInputsAsync(IStrykerOptions options,
        RelatedSourceProjectsInfo projects, ITestRunner runner);
}

public class InitialisationProcess(
    IInputFileResolver inputFileResolver,
    IInitialBuildProcess initialBuildProcess,
    IInitialTestProcess initialTestProcess,
    ILogger<InitialisationProcess> logger = null)
    : IInitialisationProcess
{
    private readonly IInputFileResolver _inputFileResolver = inputFileResolver ?? throw new ArgumentNullException(nameof(inputFileResolver));
    private readonly IInitialBuildProcess _initialBuildProcess = initialBuildProcess ?? throw new ArgumentNullException(nameof(initialBuildProcess));
    private readonly IInitialTestProcess _initialTestProcess = initialTestProcess ?? throw new ArgumentNullException(nameof(initialTestProcess));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public RelatedSourceProjectsInfo GetMutableProjectsInfo(IStrykerOptions options)
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
    public void BuildProjects(IStrykerOptions options, RelatedSourceProjectsInfo projects)
    {
        var solutionInfo = projects.Tracker;
        // pick configuration and platform from solution if available
        // we build the whole solution if we have a solution file path, even in project mode
        if (!string.IsNullOrEmpty(solutionInfo.SolutionFilePath))
        {
            solutionInfo.BuildSolution(_initialBuildProcess, projects.SourceProjectInfos.Select(p => p.AnalyzerResult));
        }
        else
        {
            // build every test projects
            var testProjects = projects.SourceProjectInfos.SelectMany(p => p.TestProjectsInfo.AnalyzerResults).Distinct().ToList();
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
                    testProjects[i].GetProperty("Configuration"),
                    testProjects[i].GetProperty("Platform"),
                    msbuildPath: options.MsBuildPath ?? testProjects[i].MsBuildPath());
            }
        }

        // perform post build update (to capture some content files in C# project for example)
        foreach (var project in projects.SourceProjectInfos)
        {
            project.OnProjectBuilt?.Invoke();
        }
    }

    public async Task<IReadOnlyCollection<MutationTestInput>> GetMutationTestInputsAsync(IStrykerOptions options,
        RelatedSourceProjectsInfo projects,
        ITestRunner runner)
    {
        var getInputs = projects.SourceProjectInfos.Select(async info => new MutationTestInput {
            SourceProjectInfo = info,
            TestRunner = runner,
            InitialTestRun = await InitialTestAsync(options, info, runner, projects.SourceProjectInfos.Count == 1)
        });
        return await Task.WhenAll(getInputs);
    }

    private async Task<InitialTestRun> InitialTestAsync(IStrykerOptions options, SourceProjectInfo projectInfo,
        ITestRunner testRunner, bool throwIfFails)
    {
        DiscoverTests(projectInfo, testRunner);

        // initial test
        _logger.LogInformation(
            "Number of tests found: {TestCount} for project {ProjectFilePath}. Initial test run started.",
        testRunner.GetTests(projectInfo).Count,
        projectInfo.AnalyzerResult.ProjectFilePath);

        var result = await _initialTestProcess.InitialTestAsync(options, projectInfo, testRunner);

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
                "{FailingTestsCount} tests are failing. Stryker will continue but outcome will be impacted.",
                failingTestsCount);
        }

        if (!result.Result.ExecutedTests.IsEmpty || !throwIfFails)
        {
            return result;
        }

        const string Message = "No test result reported. Make sure your test project contains test and is compatible with VsTest.";
        throw new InputException(string.Join(Environment.NewLine, projectInfo.Warnings.Prepend(Message)));
    }

    private static readonly Dictionary<string, (string assembly, string package)> TestFrameworks = new()
    {
        ["xunit.core"] = ("xunit.runner.visualstudio", "xunit.runner.visualstudio"),
        ["nunit.framework"] = ("NUnit3.TestAdapter", "NUnit3TestAdapter"),
        ["Microsoft.VisualStudio.TestPlatform.TestFramework"] =
                ("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter", "MSTest.TestAdapter")
    };

    private void DiscoverTests(SourceProjectInfo projectInfo, ITestRunner testRunner)
    {
        foreach (var testProject in projectInfo.TestProjectsInfo.AnalyzerResults)
        {
            if (testRunner.DiscoverTestsAsync(testProject.GetAssemblyPath()).GetAwaiter().GetResult())
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
                    message += $" This may be because the test adapter package, {package}, failed to deploy or run. " +
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

            if (!causeFound && testProject.References.Any(r => r.Contains("Microsoft.Testing.Platform")))
            {
                causeFound = true;
                var message = $"Project '{testProject.ProjectFilePath}' is using Microsoft.Testing.Platform which is not yet supported by Stryker, " +
                              $"see https://github.com/stryker-mutator/stryker-net/issues/3094";
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
