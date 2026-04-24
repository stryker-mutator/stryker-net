using Shouldly;

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
