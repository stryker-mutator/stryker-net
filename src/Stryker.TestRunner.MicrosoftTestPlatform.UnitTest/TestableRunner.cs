using Microsoft.Extensions.Logging.Abstractions;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
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
/// A server fake standing in for a real <see cref="AssemblyTestServer"/>: the base constructor opens
/// the control files but starts no process, so faking the run is safe. Each constructor call queues
/// one run's updates; extra runs return empty results.
/// </summary>
internal sealed class FakeAssemblyServer : AssemblyTestServer
{
    private readonly List<TestNodeUpdate>[] _runs;
    private int _runIndex;

    public FakeAssemblyServer(params List<TestNodeUpdate>[] runs)
        : base("fake.dll", new Dictionary<string, string?>(), NullLogger.Instance, "MtpRunner-0")
    {
        _runs = runs;
    }

    public int RunCalls { get; private set; }

    public string[] TestsSeen { get; private set; } = [];

    /// <summary>
    /// When set, the first run throws it, like a test host crash tearing down the RPC connection.
    /// </summary>
    public Exception? FirstRunFailure { get; init; }

    /// <summary>
    /// When true, every run reports the timeout flag, like a host that never finishes the run.
    /// </summary>
    public bool TimedOutRuns { get; init; }

    public override bool IsAlive => true;

    public override Task<(List<TestNodeUpdate> Results, bool TimedOut)> RunTestsAsync(TestNode[]? testsToRun, TimeSpan? timeout)
    {
        RunCalls++;
        if (RunCalls == 1 && FirstRunFailure is not null)
        {
            throw FirstRunFailure;
        }

        TestsSeen = testsToRun?.Select(t => t.Uid).ToArray() ?? [];
        var results = _runIndex < _runs.Length ? _runs[_runIndex] : new List<TestNodeUpdate>();
        _runIndex++;
        return Task.FromResult((results, TimedOutRuns));
    }
}

/// <summary>
/// A runner whose every server lookup returns the one <see cref="FakeAssemblyServer"/>, so the run
/// loop and the per-test coverage capture can be driven without a real test host.
/// </summary>
internal sealed class FakeServerLoopRunner : MicrosoftTestingPlatformRunner
{
    private readonly FakeAssemblyServer _server;

    public FakeServerLoopRunner(FakeAssemblyServer server, IStrykerOptions? options = null, Dictionary<string, List<TestNode>>? testsByAssembly = null)
        : base(0, testsByAssembly ?? new Dictionary<string, List<TestNode>>(), new Dictionary<string, MtpTestDescription>(), new TestSet(), new object(), NullLogger.Instance, options)
    {
        _server = server;
    }

    public int WarmUpCalls { get; private set; }

    // The runner keeps its servers in a private cache; faking the lookup hands every request the
    // same fake, so fresh-host behavior is observable through the warm-up skip, not the cache.
    internal override Task<AssemblyTestServer> GetOrCreateServerAsync(string assembly) =>
        Task.FromResult<AssemblyTestServer>(_server);

    internal override Task<(List<TestNodeUpdate> Updates, bool TimedOut)> RunWarmUpTestsAsync(AssemblyTestServer server, TestNode[]? tests, TimeSpan? timeout)
    {
        WarmUpCalls++;
        return Task.FromResult((new List<TestNodeUpdate>(), false));
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
