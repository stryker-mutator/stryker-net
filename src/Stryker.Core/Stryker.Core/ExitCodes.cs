namespace Stryker.Core;

/// <summary>
/// Known Stryker exit codes.
/// </summary>
public static class ExitCodes
{
    /// <summary>
    /// Successful execution.
    /// </summary>
    public static int Success = 0;

    /// <summary>
    /// Anything bad happened for reasons besides the ones specified below.
    /// </summary>
    public static int OtherError = 1;

    /// <summary>
    /// Mutation score is below the break threshold.
    /// </summary>
    public static int BreakThresholdViolated = 2;
}
