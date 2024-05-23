using Stryker.Shared.Tests;

namespace Stryker.Shared.Mutants;

/// <summary>
/// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify mutants.
/// </summary>
public interface IReadOnlyMutant
{
    int Id { get; }
    IMutation Mutation { get; }
    MutantStatus ResultStatus { get; }
    string ResultStatusReason { get; }
    ITestIdentifiers CoveringTests { get; }
    ITestIdentifiers KillingTests { get; }
    ITestIdentifiers AssessingTests { get; }
    bool CountForStats { get; }
    bool IsStaticValue { get; }
}
