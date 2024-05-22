using Stryker.Shared.Tests;

namespace Stryker.Shared.Coverage;
public interface ICoverageRunResult
{
    Identifier TestId { get; }
    IReadOnlyCollection<int> MutationsCovered { get; }
    MutationTestingRequirements this[int mutation] { get; }
    CoverageConfidence Confidence { get; }
    void Merge(ICoverageRunResult coverageRunResult);
}
