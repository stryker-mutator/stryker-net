namespace Stryker.CLI
{
    /// <summary>
    /// Known Stryker error codes.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// Anything bad happened for reasons besides the ones specified below.
        /// </summary>
        public static int OtherError = 1;

        /// <summary>
        /// Mutation score is below the break threshold.
        /// </summary>
        public static int BreakThresholdViolated = 2;
    }
}
