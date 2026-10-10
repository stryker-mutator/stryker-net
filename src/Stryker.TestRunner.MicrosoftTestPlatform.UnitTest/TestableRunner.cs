using Microsoft.Extensions.Logging.Abstractions;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

internal class TestableRunner : MicrosoftTestingPlatformRunner
{
    private readonly Action _onDispose;
    private readonly Func<string, TestNode, string, Task<ICoverageRunResult>>? _coverageHandler;
    private readonly Func<string, TestNode, string, Task<ICoverageRunResult>>? _isolatedCoverageHandler;

    public TestableRunner(int id, Action onDispose)
        : base(id, new Dictionary<string, List<TestNode>>(),
                new Dictionary<string, MtpTestDescription>(),
                new TestSet(),
                new object(),
                NullLogger.Instance)
    {
        _onDispose = onDispose;
    }

    public TestableRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        Action onDispose,
        Func<string, TestNode, string, Task<ICoverageRunResult>>? coverageHandler = null,
        Func<string, TestNode, string, Task<ICoverageRunResult>>? isolatedCoverageHandler = null)
        : base(id, testsByAssembly, testDescriptions, testSet, discoveryLock, NullLogger.Instance)
    {
        _onDispose = onDispose;
        _coverageHandler = coverageHandler;
        _isolatedCoverageHandler = isolatedCoverageHandler;
    }

    internal override async Task<ICoverageRunResult> RunSingleTestForCoverageInReusedProcessAsync(
        string assembly, TestNode test, string testId)
    {
        if (_coverageHandler is not null)
        {
            try
            {
                return await _coverageHandler(assembly, test, testId).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
                    Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
            }
        }

        return CoverageRunResult.Create(testId, CoverageConfidence.Normal,
            Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
    }

    internal override async Task<ICoverageRunResult> RunSingleTestForCoverageInIsolatedProcessAsync(
        string assembly, TestNode test, string testId)
    {
        if (_isolatedCoverageHandler is not null)
        {
            try
            {
                return await _isolatedCoverageHandler(assembly, test, testId).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Mirrors the real runner: a failing isolated capture degrades to Dubious rather than
                // tearing down the whole coverage phase.
                return CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
                    Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
            }
        }

        return CoverageRunResult.Create(testId, CoverageConfidence.Exact,
            Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
    }

    public override void Dispose(bool disposing)
    {
        _onDispose?.Invoke();
        base.Dispose(disposing);
    }
}

/// <summary>
/// Returns a failing test run for every mutant session without starting MTP hosts.
/// </summary>
internal sealed class FailingMutantSessionRunner : MicrosoftTestingPlatformRunner
{
    private readonly string _failingTestUid;

    public FailingMutantSessionRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        string failingTestUid)
        : base(id, testsByAssembly, testDescriptions, testSet, discoveryLock, NullLogger.Instance)
    {
        _failingTestUid = failingTestUid;
    }

    internal override Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> RunAssemblyTestsAsync(
        string assembly,
        ITimeoutValueCalculator? timeoutCalc,
        IReadOnlyList<IMutant>? mutants = null,
        Func<TestNode, bool>? testUidFilter = null)
    {
        var discovered = new List<TestNode> { new(_failingTestUid, _failingTestUid, "test", TestNodeStates.Discovered) };
        var result = new TestRunResult(
            Array.Empty<MtpTestDescription>(),
            new TestIdentifierList(new[] { _failingTestUid }),
            new TestIdentifierList(new[] { _failingTestUid }),
            TestIdentifierList.NoTest(),
            string.Empty,
            [],
            TimeSpan.Zero);

        return Task.FromResult<(TestRunResult?, bool, List<TestNode>?)>((result, false, discovered));
    }
}

/// <summary>
/// Delays <see cref="MicrosoftTestingPlatformRunner.RunAssemblyTestsAsync"/> so concurrent
/// <see cref="MicrosoftTestingPlatformRunner.RunAllTestsAsync"/> calls can be observed.
/// </summary>
internal sealed class SlowAssemblySessionRunner : MicrosoftTestingPlatformRunner
{
    private readonly Action onEnter;
    private readonly Action onExit;

    public SlowAssemblySessionRunner(int id, Action onEnter, Action onExit)
        : base(id, new Dictionary<string, List<TestNode>>(),
            new Dictionary<string, MtpTestDescription>(),
            new TestSet(),
            new object(),
            NullLogger.Instance)
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
    }

    internal override async Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> RunAssemblyTestsAsync(
        string assembly,
        ITimeoutValueCalculator? timeoutCalc,
        IReadOnlyList<IMutant>? mutants = null,
        Func<TestNode, bool>? testUidFilter = null)
    {
        onEnter();
        try
        {
            await Task.Delay(75).ConfigureAwait(false);
            var result = new TestRunResult(
                Array.Empty<MtpTestDescription>(),
                TestIdentifierList.NoTest(),
                TestIdentifierList.NoTest(),
                TestIdentifierList.NoTest(),
                string.Empty,
                [],
                TimeSpan.Zero);
            return (result, false, []);
        }
        finally
        {
            onExit();
        }
    }
}