namespace Stryker.Core.Mutants
{
    public enum MutantStatus
    {
        NotRun,
        Killed,
        Survived,
        Timeout,
        NoCoverage,
        CompileError,
        Skipped
    }
}
