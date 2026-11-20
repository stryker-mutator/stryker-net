using System;
using System.Collections.Concurrent;
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

    /// <summary>
    /// Discovers the tests and creates the mutation test inputs, without running the initial tests.
    /// The initial tests can be run later via <see cref="RunInitialTestsAsync"/>, allowing mutating to run in parallel with them.
    /// </summary>
    IReadOnlyCollection<MutationTestInput> GetMutationTestInputs(IStrykerOptions options,
        RelatedSourceProjectsInfo projects, ITestRunner runner);

    /// <summary>
    /// Runs the initial tests for all mutation test inputs and returns a dictionary mapping the input to its test run result.
    /// Inputs whose tests are all filtered out by the test case filter are not part of the result.
    /// </summary>
    Task<IReadOnlyDictionary<MutationTestInput, InitialTestRun>> RunInitialTestsAsync(IStrykerOptions options,
        IReadOnlyCollection<MutationTestInput> inputs);
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

    private interface IProjectPreparation
    {
    }
    private sealed record ReadyProject(MutationTestInput Input) : IProjectPreparation;
    private sealed record SkippedByTestCaseFilter : IProjectPreparation;

    private readonly IInputFileResolver _inputFileResolver = inputFileResolver ?? throw new ArgumentNullException(nameof(inputFileResolver));
    private readonly IInitialBuildProcess _initialBuildProcess = initialBuildProcess ?? throw new ArgumentNullException(nameof(initialBuildProcess));
    private readonly IInitialTestProcess _initialTestProcess = initialTestProcess ?? throw new ArgumentNullException(nameof(initialTestProcess));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Results of the discovery phase (GetMutationTestInputs), needed by the initial test phase (RunInitialTestsAsync).
    /// </summary>
    private readonly ConcurrentDictionary<MutationTestInput, (bool HasDiscoveredTests, bool ThrowIfFails)> _preparationStates = new();

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

    /// <inheritdoc/>
    public IReadOnlyCollection<MutationTestInput> GetMutationTestInputs(IStrykerOptions options,
        RelatedSourceProjectsInfo projects, ITestRunner runner) =>
        DiscoverProjectsAsync(options, projects, runner).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public async Task<IReadOnlyDictionary<MutationTestInput, InitialTestRun>> RunInitialTestsAsync(IStrykerOptions options,
        IReadOnlyCollection<MutationTestInput> inputs)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = await Task.WhenAll(inputs.Select(async input =>
        {
            var (hasDiscoveredTests, throwIfFails) = PreparationStateFor(input, inputs.Count);
            var initialTestRun = await RunInitialTestAsync(options, input, hasDiscoveredTests, throwIfFails);
            return (input, initialTestRun);
        }));
        stopwatch.Stop();

        var testedInputs = results.Where(r => r.initialTestRun is not null).ToList();
        var numberOfTests = testedInputs.Sum(r => r.initialTestRun!.Result.ExecutedTests.Count);
        _logger.LogInformation("Initial testrun ran {NumberOfTests} tests in {ElapsedMilliseconds} ms", numberOfTests, stopwatch.ElapsedMilliseconds);

        if (inputs.Count > 0 && testedInputs.Count == 0)
        {
            throw new InputException(NoMatchingProjectsMessage);
        }

        return testedInputs.ToDictionary(r => r.input, r => r.initialTestRun!);
    }

    /// <summary>
    /// Convenience method combining test discovery and the initial test run, for callers that need both in a single call.
    /// </summary>
    public async Task<IReadOnlyCollection<MutationTestInput>> GetMutationTestInputsAsync(IStrykerOptions options,
        RelatedSourceProjectsInfo projects,
        ITestRunner runner)
    {
        var inputs = await DiscoverProjectsAsync(options, projects, runner);
        var initialTestRuns = await RunInitialTestsAsync(options, inputs);

        var readyInputs = new List<MutationTestInput>();
        foreach (var input in inputs)
        {
            if (initialTestRuns.TryGetValue(input, out var initialTestRun))
            {
                input.InitialTestRun = initialTestRun;
                readyInputs.Add(input);
            }
        }
        return readyInputs;
    }

    private async Task<IReadOnlyCollection<MutationTestInput>> DiscoverProjectsAsync(IStrykerOptions options,
        RelatedSourceProjectsInfo projects, ITestRunner runner)
    {
        var preparations = await Task.WhenAll(projects.SourceProjectInfos.Select(info =>
            PrepareProjectAsync(options, info, runner, projects.SourceProjectInfos.Count == 1)));
        var inputs = preparations.OfType<ReadyProject>().Select(project => project.Input).ToList();
        if (preparations.Length > 0 && inputs.Count == 0)
        {
            throw new InputException(NoMatchingProjectsMessage);
        }

        return inputs;
    }

    private async Task<IProjectPreparation> PrepareProjectAsync(IStrykerOptions options, SourceProjectInfo projectInfo,
        ITestRunner testRunner, bool throwIfFails)
    {
        var hasDiscoveredTests = await DiscoverTestsAsync(options, projectInfo, testRunner);
        var hasTestCaseFilter = !string.IsNullOrWhiteSpace(options.TestCaseFilter);
        if (!hasDiscoveredTests && hasTestCaseFilter && projectInfo.TestProjectsInfo.AnalyzerResults.Any())
        {
            LogFilteredProject(projectInfo);
            return new SkippedByTestCaseFilter();
        }

        var input = new MutationTestInput
        {
            SourceProjectInfo = projectInfo,
            TestRunner = testRunner
        };
        _preparationStates[input] = (hasDiscoveredTests, throwIfFails);
        return new ReadyProject(input);
    }

    private async Task<InitialTestRun?> RunInitialTestAsync(IStrykerOptions options, MutationTestInput input,
        bool hasDiscoveredTests, bool throwIfFails)
    {
        var projectInfo = input.SourceProjectInfo;
        var testRunner = input.TestRunner;
        var hasTestCaseFilter = !string.IsNullOrWhiteSpace(options.TestCaseFilter);

        // initial test
        var foundTests = testRunner.GetTests(projectInfo).Count;
        _logger.LogInformation(
            "Number of tests found: {TestCount} for project {ProjectFilePath}. Initial test run started.",
        foundTests,
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
            return null;
        }

        if (!result.Result.FailingTests.IsEmpty)
        {
            AnalyzeTestFailures(options, throwIfFails, result, foundTests);
        }

        if (!result.Result.ExecutedTests.IsEmpty || !throwIfFails)
        {
            return result;
        }

        var runnerName = testRunner.RunnerType switch
        {
            Abstractions.Options.TestRunner.MicrosoftTestPlatform => "Microsoft Testing Platform",
            Abstractions.Options.TestRunner.VsTest => "VsTest",
            _ => "Unknown Test Runner"
        };

        var message = $"No test result reported. Make sure your test project contains tests and is compatible with {runnerName}.";
        throw new InputException(string.Join(Environment.NewLine, projectInfo.Warnings.Prepend(message)));
    }

    private (bool HasDiscoveredTests, bool ThrowIfFails) PreparationStateFor(MutationTestInput input, int inputCount) =>
        _preparationStates.TryGetValue(input, out var state) ? state : (true, inputCount == 1);

    private void AnalyzeTestFailures(IStrykerOptions options, bool throwIfFails, InitialTestRun result, int foundTests)
    {
        var failingTestsCount = result.Result.FailingTests.Count;
        if (options.BreakOnInitialTestFailure)
        {
            throw new InputException("Initial testrun has failing tests.", result.Result.ResultMessage);
        }

        if (result.Result.ExecutedTests.Count != 0)
        {
            foundTests = result.Result.ExecutedTests.Count;
        }

        if (throwIfFails && (double)failingTestsCount / foundTests >= .5)
        {
            throw new InputException("Initial testrun has 50% or more failing tests.",
                result.Result.ResultMessage);
        }

        _logger.LogWarning(
            "{FailingTestsCount} tests are failing. Stryker will continue but outcome will be impacted.",
            failingTestsCount);
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
