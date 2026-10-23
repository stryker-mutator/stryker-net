using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class TestNodeStatesTests
{
    [TestMethod]
    [DataRow(TestNodeStates.Passed, true)]
    [DataRow(TestNodeStates.Skipped, true)]
    [DataRow(TestNodeStates.Failed, true)]
    [DataRow(TestNodeStates.Error, true)]
    [DataRow(TestNodeStates.TimedOut, true)]
    [DataRow(TestNodeStates.Cancelled, true)]
    [DataRow(TestNodeStates.Discovered, false)]
    [DataRow(TestNodeStates.InProgress, false)]
    [DataRow(null, false)]
    public void IsFinished_ReturnsExpected(string? state, bool expected) =>
        TestNodeStates.IsFinished(state).ShouldBe(expected);

    [TestMethod]
    [DataRow(TestNodeStates.Failed, true)]
    [DataRow(TestNodeStates.Error, true)]
    [DataRow(TestNodeStates.Cancelled, true)]
    [DataRow(TestNodeStates.Passed, false)]
    [DataRow(TestNodeStates.Skipped, false)]
    [DataRow(TestNodeStates.TimedOut, false)]
    [DataRow(TestNodeStates.Discovered, false)]
    [DataRow(TestNodeStates.InProgress, false)]
    [DataRow(null, false)]
    public void IsFailure_ReturnsExpected(string? state, bool expected) =>
        TestNodeStates.IsFailure(state).ShouldBe(expected);

    [TestMethod]
    [DataRow(TestNodeStates.TimedOut, true)]
    [DataRow(TestNodeStates.Failed, false)]
    [DataRow(TestNodeStates.Error, false)]
    [DataRow(TestNodeStates.Cancelled, false)]
    [DataRow(TestNodeStates.Passed, false)]
    [DataRow(TestNodeStates.Skipped, false)]
    [DataRow(TestNodeStates.Discovered, false)]
    [DataRow(TestNodeStates.InProgress, false)]
    [DataRow(null, false)]
    public void IsTimeout_ReturnsExpected(string? state, bool expected) =>
        TestNodeStates.IsTimeout(state).ShouldBe(expected);

    [TestMethod]
    public void StateConstants_MatchWireFormat()
    {
        TestNodeStates.Discovered.ShouldBe("discovered");
        TestNodeStates.InProgress.ShouldBe("in-progress");
        TestNodeStates.Passed.ShouldBe("passed");
        TestNodeStates.Skipped.ShouldBe("skipped");
        TestNodeStates.Failed.ShouldBe("failed");
        TestNodeStates.Error.ShouldBe("error");
        TestNodeStates.TimedOut.ShouldBe("timed-out");
        TestNodeStates.Cancelled.ShouldBe("cancelled");
    }

    [TestMethod]
    public void CollapseFinishedUpdates_FailureWinsOverPassedForSameUid()
    {
        var passed = new TestNodeUpdate(
            new TestNode("t1", "t1", "test", TestNodeStates.Passed),
            "root");
        var failed = new TestNodeUpdate(
            new TestNode("t1", "t1", "test", TestNodeStates.Failed),
            "root");

        var collapsed = TestNodeStates.CollapseFinishedUpdates([passed, failed]);

        collapsed.Count.ShouldBe(1);
        collapsed[0].Node.ExecutionState.ShouldBe(TestNodeStates.Failed);
    }

    [TestMethod]
    public void CollapseFinishedUpdates_TimeoutWinsOverPassedForSameUid()
    {
        var passed = new TestNodeUpdate(
            new TestNode("t1", "t1", "test", TestNodeStates.Passed),
            "root");
        var timedOut = new TestNodeUpdate(
            new TestNode("t1", "t1", "test", TestNodeStates.TimedOut),
            "root");

        var collapsed = TestNodeStates.CollapseFinishedUpdates([passed, timedOut]);

        collapsed.Count.ShouldBe(1);
        collapsed[0].Node.ExecutionState.ShouldBe(TestNodeStates.TimedOut);
    }

    private static TestNodeUpdate Update(string uid, string state) =>
        new(new TestNode(uid, uid, "test", state), "root");

    [TestMethod]
    public void CollapseFinishedUpdates_ReturnsNothing_ForNoUpdates() =>
        TestNodeStates.CollapseFinishedUpdates([]).ShouldBeEmpty();

    [TestMethod]
    public void CollapseFinishedUpdates_IgnoresUpdatesThatAreNotFinished()
    {
        var collapsed = TestNodeStates.CollapseFinishedUpdates(
            [Update("t1", TestNodeStates.Discovered), Update("t2", TestNodeStates.InProgress), Update("t3", TestNodeStates.Passed)]);

        collapsed.Select(update => update.Node.Uid).ShouldBe(["t3"]);
    }

    [TestMethod]
    public void CollapseFinishedUpdates_FailureStaysWhenAPassedUpdateArrivesLater()
    {
        var collapsed = TestNodeStates.CollapseFinishedUpdates([Update("t1", TestNodeStates.Failed), Update("t1", TestNodeStates.Passed)]);

        collapsed.Count.ShouldBe(1);
        collapsed[0].Node.ExecutionState.ShouldBe(TestNodeStates.Failed);
    }

    [TestMethod]
    public void CollapseFinishedUpdates_FailureWinsOverTimeout()
    {
        var collapsed = TestNodeStates.CollapseFinishedUpdates([Update("t1", TestNodeStates.TimedOut), Update("t1", TestNodeStates.Error)]);

        collapsed.Count.ShouldBe(1);
        collapsed[0].Node.ExecutionState.ShouldBe(TestNodeStates.Error);
    }

    [TestMethod]
    public void CollapseFinishedUpdates_KeepsTheStrongestOfThreeUpdatesForOneUid()
    {
        var collapsed = TestNodeStates.CollapseFinishedUpdates(
            [Update("t1", TestNodeStates.Passed), Update("t1", TestNodeStates.TimedOut), Update("t1", TestNodeStates.Skipped)]);

        collapsed.Count.ShouldBe(1);
        collapsed[0].Node.ExecutionState.ShouldBe(TestNodeStates.TimedOut);
    }

    [TestMethod]
    public void CollapseFinishedUpdates_KeepsDistinctUidsApart()
    {
        var collapsed = TestNodeStates.CollapseFinishedUpdates(
            [Update("t1", TestNodeStates.Passed), Update("t2", TestNodeStates.Failed), Update("t1", TestNodeStates.Passed)]);

        collapsed.Select(update => update.Node.Uid).OrderBy(uid => uid).ShouldBe(["t1", "t2"]);
    }

    [TestMethod]
    public void TestNode_SerializesErrorDetailsUnderTheirWireNames()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(
            new TestNode("uid", "Test", "action", TestNodeStates.Error, ErrorMessage: "boom", ErrorStackTrace: "at X"));

        json.ShouldContain("\"error.message\":\"boom\"");
        json.ShouldContain("\"error.stacktrace\":\"at X\"");
    }

    [TestMethod]
    public void TestNode_OmitsErrorDetails_WhenThereAreNone()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(new TestNode("uid", "Test", "action", TestNodeStates.Passed));

        json.ShouldNotContain("error.message");
        json.ShouldNotContain("error.stacktrace");
    }

    [TestMethod]
    [DataRow(TestNodeStates.Error, "System.TypeInitializationException : The type initializer for 'X' threw an exception.", null, true)]
    [DataRow(TestNodeStates.Failed, "System.TypeLoadException : Could not load type 'X'.", null, true)]
    [DataRow(TestNodeStates.Failed, "boom", "   at X..cctor()\n System.TypeInitializationException: ...", true)]
    [DataRow(TestNodeStates.Failed, "Assert.Equal() Failure", "at Tests.Foo()", false)]
    [DataRow(TestNodeStates.Failed, null, null, false)]
    [DataRow(TestNodeStates.Passed, "System.TypeInitializationException", null, false)]
    [DataRow(TestNodeStates.TimedOut, "System.TypeInitializationException", null, false)]
    [DataRow(TestNodeStates.Cancelled, "System.TypeInitializationException", null, true)]
    [DataRow(TestNodeStates.Failed, "System.Reflection.ReflectionTypeLoadException : Unable to load types", null, true)]
    [DataRow(TestNodeStates.Failed, "system.typeinitializationexception", null, false)]
    [DataRow(TestNodeStates.Failed, "", "", false)]
    [DataRow(TestNodeStates.Discovered, "System.TypeLoadException", null, false)]
    public void IsHostPoisoning_ReturnsExpected(string state, string? message, string? stackTrace, bool expected)
    {
        var node = new TestNode("uid", "Test", "action", state, ErrorMessage: message, ErrorStackTrace: stackTrace);

        TestNodeStates.IsHostPoisoning(node).ShouldBe(expected);
    }

    [TestMethod]
    public void StateClassification_IsCaseSensitive()
    {
        // Defence in depth: the MTP wire format is lowercase, so we don't
        // normalise, but make that contract explicit so a future unintentional
        // case-insensitive change is caught.
        TestNodeStates.IsFailure("Failed").ShouldBeFalse();
        TestNodeStates.IsFailure("ERROR").ShouldBeFalse();
        TestNodeStates.IsTimeout("Timed-Out").ShouldBeFalse();
    }
}
