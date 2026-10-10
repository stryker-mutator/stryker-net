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
    private const string NoMatchingTestsMessage =
        "No test cases matched `test-case-filter`. Skipping this project. Change your configuration and try again.";
    private const string NoMatchingProjectsMessage =
        "No projects have test cases matching `test-case-filter`. Change your configuration and try again.";

    private abstract record ProjectPreparation;
    private sealed record ReadyProject(MutationTestInput Input) : ProjectPreparation;
    private sealed record SkippedByTestCaseFilter : ProjectPreparation;

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
        // ensure test projects are built
        projects.BuildTestProjects(_initialBuildProcess);
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
        var prepareProjects = projects.SourceProjectInfos.Select(info =>
            PrepareProjectAsync(options, info, runner, projects.SourceProjectInfos.Count == 1));
        var preparations = await Task.WhenAll(prepareProjects);
        var inputs = preparations.OfType<ReadyProject>().Select(project => project.Input).ToList();
        if (preparations.Length > 0 && preparations.All(project => project is SkippedByTestCaseFilter))
        {
            throw new InputException(NoMatchingProjectsMessage);
        }

        return inputs;
    }

    private async Task<ProjectPreparation> PrepareProjectAsync(IStrykerOptions options, SourceProjectInfo projectInfo,
        ITestRunner testRunner, bool throwIfFails)
    {
        var hasDiscoveredTests = await DiscoverTestsAsync(options, projectInfo, testRunner);
        var hasTestCaseFilter = !string.IsNullOrWhiteSpace(options.TestCaseFilter);
        if (!hasDiscoveredTests && hasTestCaseFilter && projectInfo.TestProjectsInfo.AnalyzerResults.Any())
        {
            LogFilteredProject(projectInfo);
            return new SkippedByTestCaseFilter();
        }

        // initial test
        _logger.LogInformation(
            "Number of tests found: {TestCount} for project {ProjectFilePath}. Initial test run started.",
        testRunner.GetTests(projectInfo).Count,
        projectInfo.AnalyzerResult.ProjectFilePath);

        var result = await _initialTestProcess.InitialTestAsync(options, projectInfo, testRunner);

        if (result.Result.SessionTimedOut || result.Result.SessionHadRuntimeIssue || !result.Result.TimedOutTests.IsEmpty)
        {
            throw new InputException("Initial test run could not be completed.", result.Result.ResultMessage);
        }

        if (result.Result.ExecutedTests.IsEmpty && hasTestCaseFilter &&
            result.Result.FailingTests.IsEmpty && hasDiscoveredTests)
        {
            LogFilteredProject(projectInfo);
            return new SkippedByTestCaseFilter();
        }

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
            return new ReadyProject(new MutationTestInput
            {
                SourceProjectInfo = projectInfo,
                TestRunner = testRunner,
                InitialTestRun = result
            });
        }

        var runnerName = options.TestRunner == Abstractions.Options.TestRunner.MicrosoftTestPlatform
            ? "Microsoft Testing Platform"
            : "VsTest";
        var message = $"No test result reported. Make sure your test project contains tests and is compatible with {runnerName}.";
        throw new InputException(string.Join(Environment.NewLine, projectInfo.Warnings.Prepend(message)));
    }

    private void LogFilteredProject(SourceProjectInfo projectInfo) =>
        _logger.LogWarning("{Message} Project: {ProjectFilePath}", NoMatchingTestsMessage, projectInfo.AnalyzerResult.ProjectFilePath);

    private static readonly Dictionary<string, (string assembly, string package)> TestFrameworks = new()
    {
        ["xunit.core"] = ("xunit.runner.visualstudio", "xunit.runner.visualstudio"),
        ["nunit.framework"] = ("NUnit3.TestAdapter", "NUnit3TestAdapter"),
        ["Microsoft.VisualStudio.TestPlatform.TestFramework"] =
                ("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter", "MSTest.TestAdapter")
    };

    private async Task<bool> DiscoverTestsAsync(IStrykerOptions options, SourceProjectInfo projectInfo, ITestRunner testRunner)
    {
        var hasDiscoveredTests = false;
        foreach (var testProject in projectInfo.TestProjectsInfo.AnalyzerResults)
        {
            if (await testRunner.DiscoverTestsAsync(testProject.GetAssemblyPath()))
            {
                hasDiscoveredTests = true;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(options.TestCaseFilter))
            {
                continue;
            }

            var causeFound = false;
            foreach (var (framework, (adapter, package)) in
                     TestFrameworks.Where(t => options.TestRunner == Abstractions.Options.TestRunner.VsTest &&
                         testProject.References.Any(r => r.Contains(t.Key))))
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

            if (causeFound)
            {
                continue;
            }

            var messageForNoReason = $"No test detected for project '{testProject.ProjectFilePath}'. No cause identified.";
            projectInfo.LogError(messageForNoReason);
            _logger.LogWarning(messageForNoReason);
        }

        return hasDiscoveredTests;
    }
}
