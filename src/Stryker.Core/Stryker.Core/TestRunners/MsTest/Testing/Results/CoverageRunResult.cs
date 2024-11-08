using System;
using System.Collections.Generic;

namespace Stryker.Core.TestRunners.MsTest.Testing.Results;

internal class CoverageRunResult : ICoverageRunResult
{
    private readonly Dictionary<int, MutationTestingRequirements> _mutationFlags = [];

    private CoverageRunResult(string testId, CoverageConfidence confidence, IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations, IEnumerable<int> leakedMutations)
    {
        TestId = Identifier.Create(testId);

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
            var requirement = confidence == CoverageConfidence.Exact ?
                MutationTestingRequirements.NeedEarlyActivation :
                MutationTestingRequirements.CoveredOutsideTest;

            _mutationFlags[leakedMutation] = requirement;
        }

        Confidence = confidence;
    }

    public static CoverageRunResult Create(
        string testId,
        CoverageConfidence confidence,
        IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations,
        IEnumerable<int> leakedMutations) => new(testId, confidence, coveredMutations, detectedStaticMutations, leakedMutations);

    public MutationTestingRequirements this[int mutation] => _mutationFlags.TryGetValue(mutation, out var value) ? value : MutationTestingRequirements.NotCovered;

    public Identifier TestId { get; }

    public IReadOnlyCollection<int> MutationsCovered => _mutationFlags.Keys;

    public CoverageConfidence Confidence { get; private set; }

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
