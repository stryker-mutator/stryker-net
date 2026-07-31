namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Canonical Microsoft Testing Platform <c>execution-state</c> string values and
/// classification helpers used when mapping MTP test outcomes to Stryker mutant status.
/// </summary>
/// <remarks>
/// The string values match Microsoft.Testing.Platform's wire format
/// (see its <c>SerializerUtilities</c>). Keeping them in one place avoids the
/// single-literal bug that caused tests ending in <c>"error"</c>, <c>"timed-out"</c>
/// or <c>"cancelled"</c> to be silently dropped when only <c>"failed"</c> was
/// treated as a failure — for example TUnit routes non-assertion exceptions
/// (such as NSubstitute's <c>ReceivedCallsException</c>) through its
/// <c>ErrorTestNode</c> path, producing <c>"error"</c> rather than <c>"failed"</c>.
/// </remarks>
internal static class TestNodeStates
{
    public const string Discovered = "discovered";
    public const string InProgress = "in-progress";
    public const string Passed = "passed";
    public const string Skipped = "skipped";
    public const string Failed = "failed";
    public const string Error = "error";
    public const string TimedOut = "timed-out";
    public const string Cancelled = "cancelled";

    /// <summary>
    /// True when the test has reached a terminal state (i.e. not still running or
    /// merely discovered). Used to decide which updates contribute to the
    /// executed-tests set.
    /// </summary>
    public static bool IsFinished(string? state) =>
        state is not (null or InProgress or Discovered);

    /// <summary>
    /// True when the test ended in a state that indicates the mutant changed
    /// observable behaviour: an assertion failure, a non-assertion exception,
    /// or explicit cancellation. Timeouts are reported separately via
    /// <see cref="IsTimeout"/> so the resulting mutant is classified as
    /// <c>Timeout</c> rather than <c>Killed</c>.
    /// </summary>
    public static bool IsFailure(string? state) =>
        state is Failed or Error or Cancelled;

    /// <summary>
    /// True when the test reported a per-test timeout.
    /// </summary>
    public static bool IsTimeout(string? state) =>
        state is TimedOut;
}
