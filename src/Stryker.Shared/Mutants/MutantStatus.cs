namespace Stryker.Shared.Mutants;
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
