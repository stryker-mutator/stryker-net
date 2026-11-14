using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
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
