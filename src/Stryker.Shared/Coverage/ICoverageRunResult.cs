namespace Stryker.Shared.Coverage;
public interface ICoverageRunResult
{
    Guid TestId { get; }
    IReadOnlyCollection<int> MutationsCovered { get; }
    MutationTestingRequirements this[int mutation] { get; }
    CoverageConfidence Confidence { get; }
    void Merge(ICoverageRunResult coverageRunResult);
}
