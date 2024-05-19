using Stryker.Shared.Coverage;

namespace Stryker.TestRunner.VSTest;

public class CoverageRunResult : ICoverageRunResult
{
    private readonly Dictionary<int, MutationTestingRequirements> _mutationFlags = [];

    public Guid TestId { get; }

    public IReadOnlyCollection<int> MutationsCovered => _mutationFlags.Keys;

    public MutationTestingRequirements this[int mutation] => _mutationFlags.TryGetValue(mutation, out var value) ? value : MutationTestingRequirements.NotCovered;

    public CoverageConfidence Confidence { get; private set; }

    public CoverageRunResult(Guid testId, CoverageConfidence confidence, IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations, IEnumerable<int> leakedMutations)
    {
        TestId = testId;
        foreach (var coveredMutation in coveredMutations)
        {
            _mutationFlags[coveredMutation] = MutationTestingRequirements.None;
        }

        foreach (var detectedStaticMutation in detectedStaticMutations)
        {
            _mutationFlags[detectedStaticMutation] = MutationTestingRequirements.Static;
        }

        foreach (var leakedMutation in leakedMutations)
        {
            _mutationFlags[leakedMutation] = confidence == CoverageConfidence.Exact ? MutationTestingRequirements.NeedEarlyActivation: MutationTestingRequirements.CoveredOutsideTest;
        }

        Confidence = confidence;
    }

    public void Merge(ICoverageRunResult coverageRunResult)
    {
        var coverage = (CoverageRunResult)coverageRunResult;
        Confidence = (CoverageConfidence)Math.Min((int)Confidence, (int)coverage.Confidence);
        foreach (var mutationFlag in coverage._mutationFlags)
        {
            if (_mutationFlags.ContainsKey(mutationFlag.Key))
            {
                _mutationFlags[mutationFlag.Key] |= mutationFlag.Value;
            }
            else
            {
                _mutationFlags[mutationFlag.Key] = mutationFlag.Value;
            }
        }
    }
}
