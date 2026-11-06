using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

/// <summary>
/// The assembly run loop (<c>RunAssemblyTestsInternalAsync</c>) paths that do not need a real test
/// host, and the result accumulator the loop feeds (a private class, reached by reflection).
/// </summary>
[TestClass]
public class MicrosoftTestingPlatformRunnerRunLoopTests
{
    private static TestNodeUpdate Update(string state, string? message = null, string? stackTrace = null) =>
        new(new TestNode("uid", "Test", "action", state, ErrorMessage: message, ErrorStackTrace: stackTrace), "root");

    [TestMethod]
    public async Task RunAssemblyTestsInternalAsync_RunsTheTestsAndReturnsTheResult_OnAReusedHost()
    {
        // Off mode: no warm-up - the loop must run once, recycle-if-poisoned on the plain
        // results and hand the built result back untouched.
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(o => o.OptimizationMode).Returns(OptimizationModes.None);
        using var server = new FakeAssemblyServer([Update(TestNodeStates.Passed)]);
        var tests = new Dictionary<string, List<TestNode>>
        {
            ["fake.dll"] = [new("keep", "keep", "action", TestNodeStates.Discovered), new("skip", "skip", "action", TestNodeStates.Discovered)],
        };
        using var runner = new FakeServerLoopRunner(server, options.Object, tests);

        var (result, timedOut) = await runner.RunAssemblyTestsInternalAsync("fake.dll", testUidFilter: t => t.Uid == "keep");

        timedOut.ShouldBeFalse();
        server.RunCalls.ShouldBe(1);
        server.TestsSeen.ShouldBe(["keep"], "the run must pass only the tests the filter selects");
        result.ExecutedTests.IsEmpty.ShouldBeFalse();
    }

    [TestMethod, Timeout(2000)]
    public async Task RunAssemblyTestsInternalAsync_RetriesOnceOnAFreshServer_WhenTheRunCrashes()
    {
        // A crashed test host tears down the RPC connection and the run throws; the loop must retry
        // once on a freshly looked-up server instead of surfacing the crash as the run's verdict.
        using var server = new FakeAssemblyServer([Update(TestNodeStates.Passed)]) { FirstRunFailure = new InvalidOperationException("RPC connection is down") };
        using var runner = new FakeServerLoopRunner(server);

        var (result, timedOut) = await runner.RunAssemblyTestsInternalAsync("fake.dll", testUidFilter: null);

        timedOut.ShouldBeFalse();
        server.RunCalls.ShouldBe(2, "the crashed run must be retried exactly once");
        result.FailingTests.IsEmpty.ShouldBeTrue("the retry succeeds, so the result is a judged run, not the crash sentinel");
    }

    [TestMethod, Timeout(30000)]
    public async Task RunAssemblyTestsInternalAsync_ReturnsTheCrashSentinel_WhenTheServerCannotStart()
    {
        using var runner = new MicrosoftTestingPlatformRunner(
            0, new Dictionary<string, List<TestNode>>(), new Dictionary<string, MtpTestDescription>(), new TestSet(), new object(), NullLogger.Instance);

        var (result, timedOut) = await runner.RunAssemblyTestsInternalAsync("/nonexistent/assembly.dll", testUidFilter: null);

        timedOut.ShouldBeFalse();
        result.FailingTests.IsEveryTest.ShouldBeTrue("the crash sentinel flags the run so its mutants are classified RuntimeError");
        result.ResultMessage.ShouldNotBeNullOrWhiteSpace("the sentinel carries the start-failure message");
    }

    [TestMethod]
    public void Aggregate_RecordsTheResultMessage_OfANormalRun()
    {
        // A normal (non-crashed) run that carries a result message must surface it in the aggregate
        // error message too; until now only the crash-sentinel path was exercised.
        var accumulatorType = typeof(MicrosoftTestingPlatformRunner).GetNestedType(
            "TestRunAccumulator", System.Reflection.BindingFlags.NonPublic)!;
        var accumulator = Activator.CreateInstance(accumulatorType);

        var result = new TestRunResult(
            System.Linq.Enumerable.Empty<IFrameworkTestDescription>(),
            TestIdentifierList.NoTest(),
            new TestIdentifierList("t1"),
            TestIdentifierList.NoTest(),
            "Some tests reported errors",
            [],
            TimeSpan.FromSeconds(2));
        accumulatorType.GetMethod("Aggregate")!.Invoke(accumulator, [result, null]);

        var hasError = accumulatorType.GetProperty("HasError")!.GetValue(accumulator);
        ((bool)hasError!).ShouldBeFalse("a non-crashed run with a message is not a session error");
        accumulatorType.GetMethod("BuildErrorMessage")!.Invoke(accumulator, null)
            .ShouldBe("Some tests reported errors");
    }
}
