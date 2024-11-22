using System.Collections.Generic;

namespace Stryker.Abstractions.Testing;

public interface ICoverageRunResult
{
    Identifier TestId { get; }
    Dictionary<int, MutationTestingRequirements> MutationFlags { get; }
    IReadOnlyCollection<int> MutationsCovered { get; }
    MutationTestingRequirements this[int mutation] { get; }
    CoverageConfidence Confidence { get; }
    void Merge(ICoverageRunResult coverageRunResult);
}
