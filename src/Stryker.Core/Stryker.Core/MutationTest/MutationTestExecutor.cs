using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;
using static Stryker.Abstractions.Testing.ITestRunner;

namespace Stryker.Core.MutationTest;

/// <summary>
/// Executes exactly one mutation test and stores the result
/// </summary>
public interface IMutationTestExecutor
{
    ITestRunner TestRunner { get; set; }

    Task TestAsync(
        IProjectAndTests project,
        IList<IMutant> mutantsToTest,
        ITimeoutValueCalculator timeoutMs,
        TestUpdateHandler updateHandler,
        CancellationToken cancellationToken = default);
}

public class MutationTestExecutor : IMutationTestExecutor
{
    // Test runner can't be set in the constructor because it is determined at runtime.
    public ITestRunner TestRunner { get; set; }
    private ILogger Logger { get; }

    public MutationTestExecutor(ILogger<MutationTestExecutor> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task TestAsync(
        IProjectAndTests project,
        IList<IMutant> mutantsToTest,
        ITimeoutValueCalculator timeoutMs,
        TestUpdateHandler updateHandler,
        CancellationToken cancellationToken = default)
    {
        var forceSingle = false;
        while (mutantsToTest.Any())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await RunTestSessionAsync(
                project,
                mutantsToTest,
                timeoutMs,
                updateHandler,
                forceSingle,
                cancellationToken).ConfigureAwait(false);

            Logger.LogDebug(
                "Test run for {Mutants} is {Result} ",
                string.Join(", ", mutantsToTest.Select(x => x.DisplayName)),
                result.FailingTests.Count == 0 ? "success" : "failed");

            if (result.Messages is not null && result.Messages.Any())
            {
                Logger.LogTrace(
                    "Messages for {Mutants}: {NewLine}{Messages}",
                    string.Join(", ", mutantsToTest.Select(x => x.DisplayName)),
                    Environment.NewLine,
                    string.Join("", result.Messages));
            }

            var remainingMutants = mutantsToTest.Where((m) => m.ResultStatus == MutantStatus.Pending).ToList();
            if (remainingMutants.Count == mutantsToTest.Count)
            {
                // No mutant in this session got a conclusive result. A single mutant is already
                // classified by Mutant.AnalyzeTestRun, so reaching here means the whole batch was
                // inconclusive: rerun one mutant at a time to isolate the culprit, unless nothing
                // ran at all.
                if (result.SessionTimedOut || result.SessionHadRuntimeIssue)
                {
                    forceSingle = true;
                }
                else
                {
                    // something bad happened.
                    Logger.LogError("Stryker failed to test {RemainingMutantsCount} mutant(s).", remainingMutants.Count);
                    return;
                }
            }

            if (remainingMutants.Any())
            {
                Logger.LogDebug("Not all mutants were tested.");
            }

            mutantsToTest = remainingMutants;
        }
    }

    private async Task<ITestRunResult> RunTestSessionAsync(IProjectAndTests projectAndTests, ICollection<IMutant> mutantsToTest,
        ITimeoutValueCalculator timeoutMs,
        TestUpdateHandler updateHandler,
        bool forceSingle,
        CancellationToken cancellationToken)
    {
        Logger.LogTrace("Testing {MutantsToTest}.", string.Join(" ,", mutantsToTest.Select(x => x.DisplayName)));
        if (forceSingle)
        {
            foreach (var mutant in mutantsToTest)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var localResult = await RunMutantsAsync(
                    projectAndTests,
                    timeoutMs,
                    [mutant],
                    updateHandler,
                    cancellationToken).ConfigureAwait(false);
                if (updateHandler == null || localResult.SessionTimedOut || localResult.SessionHadRuntimeIssue)
                {
                    mutant.AnalyzeTestRun(localResult.FailingTests,
                        localResult.ExecutedTests,
                        localResult.TimedOutTests,
                        localResult.SessionTimedOut,
                        localResult.SessionHadRuntimeIssue);
                }
            }

            return new TestRunResult(true);
        }

        var result = await RunMutantsAsync(
            projectAndTests,
            timeoutMs,
            mutantsToTest.ToList(),
            updateHandler,
            cancellationToken).ConfigureAwait(false);
        if (updateHandler != null && !result.SessionTimedOut && !result.SessionHadRuntimeIssue)
        {
            return result;
        }

        foreach (var mutant in mutantsToTest)
        {
            mutant.AnalyzeTestRun(result.FailingTests,
                result.ExecutedTests,
                result.TimedOutTests,
                mutantsToTest.Count == 1 && result.SessionTimedOut,
                mutantsToTest.Count == 1 && result.SessionHadRuntimeIssue);
        }

        return result;
    }

    private Task<ITestRunResult> RunMutantsAsync(
        IProjectAndTests project,
        ITimeoutValueCalculator timeoutMs,
        IReadOnlyList<IMutant> mutants,
        TestUpdateHandler updateHandler,
        CancellationToken cancellationToken)
        => TestRunner.TestMultipleMutantsAsync(
            project,
            timeoutMs,
            mutants,
            updateHandler,
            cancellationToken);
}
