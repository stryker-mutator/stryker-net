namespace Stryker.CLI
{
    /// <summary>
    /// Known Stryker exit codes.
    /// </summary>
    public static class ExitCodes
    {
        /// <summary>
        /// Stryker is executed against .NET Framework but no solution file is given.
        /// </summary>
        public static int NoSolutionFileSpecified = 1;

        /// <summary>
        /// Mutation score is below the break threshold.
        /// </summary>
        public static int BreakThresholdViolated = 2;
    }
}
