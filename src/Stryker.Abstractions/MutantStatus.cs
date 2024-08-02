namespace Stryker.Configuration.Mutants
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
