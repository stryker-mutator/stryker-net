namespace Stryker.Core.Mutants
{
    public enum MutantStatus
    {
        NotRun,
        Killed,
        Untouched,
        Survived,
        Timeout,
        CompileError,
        Skipped
    }
}
