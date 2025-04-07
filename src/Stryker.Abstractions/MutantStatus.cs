namespace Stryker.Abstractions;

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
