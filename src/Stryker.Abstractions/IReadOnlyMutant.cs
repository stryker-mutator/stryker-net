using System;
using Stryker.Abstractions.Testing;

namespace Stryker.Abstractions;

/// <summary>
/// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify mutants.
/// </summary>
public interface IReadOnlyMutant
{
    int Id { get; }
    Mutation Mutation { get; }
    /// <summary>Gets the elapsed time spent testing this mutant.</summary>
    TimeSpan? TestDuration { get; }
    /// <summary>Gets the number of times this mutant was hit during coverage analysis.</summary>
    long? HitCount { get; }
    /// <summary>Gets the hit limit used when testing this mutant.</summary>
    long? HitLimit { get; }
    MutantStatus ResultStatus { get; }
    string ResultStatusReason { get; }
    ITestIdentifiers CoveringTests { get; }
    ITestIdentifiers KillingTests { get; }
    ITestIdentifiers AssessingTests { get; }
    bool CountForStats { get; }
    bool IsStaticValue { get; }
}
