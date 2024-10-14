namespace Stryker.Abstractions.Mutants
{
    public enum MutantStatus
    {
        Pending,
        Killed,
        Survived,
        Timeout,
        CompileError,
        Ignored,
        NoCoverage
    }
}
