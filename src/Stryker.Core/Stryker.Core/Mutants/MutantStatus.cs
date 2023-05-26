namespace Stryker.Core.Mutants
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
