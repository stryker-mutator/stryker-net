namespace Stryker.Shared.Coverage;

[Flags]
public enum MutationTestingRequirements
{
    None = 0,
    // mutation is static or executed inside à static context
    Static = 1,
    // mutation is covered outside test (before or after)
    CoveredOutsideTest = 2,
    // mutation needs to be activated ASAP when tested
    NeedEarlyActivation = 4,
    // mutation needs to be run in 'all tests' mode
    AgainstAllTests = 8,
    // not covered
    NotCovered = 256
}
