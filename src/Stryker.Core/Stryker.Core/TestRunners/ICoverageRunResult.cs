using Stryker.Shared.Tests;

namespace Stryker.Core.TestRunners;
public interface ICoverageRunResult
{
    Identifier TestId { get; }
    IReadOnlyCollection<int> MutationsCovered { get; }
    MutationTestingRequirements this[int mutation] { get; }
    CoverageConfidence Confidence { get; }
    void Merge(ICoverageRunResult coverageRunResult);
}
