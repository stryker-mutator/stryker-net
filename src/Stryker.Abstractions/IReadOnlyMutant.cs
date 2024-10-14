using Stryker.Abstractions.TestRunners;
using Stryker.Abstractions.Mutants;

namespace Stryker.Abstractions;

/// <summary>
/// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify mutants.
/// </summary>
public interface IReadOnlyMutant
{
    int Id { get; }
    Mutation Mutation { get; }
    MutantStatus ResultStatus { get; }
    string ResultStatusReason { get; }
    ITestGuids CoveringTests { get; }
    ITestGuids KillingTests { get; }
    ITestGuids AssessingTests { get; }
    bool CountForStats { get; }
    bool IsStaticValue { get; }
}
