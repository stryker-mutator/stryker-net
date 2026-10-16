using System;
using System.Collections.Generic;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.Results;

public class CoverageRunResult : ICoverageRunResult
{
    public Dictionary<int, MutationTestingRequirements> MutationFlags { get; } = new();
    public Dictionary<int, int> MutationHitCounts { get; } = new();
    IReadOnlyDictionary<int, int> ICoverageRunResult.MutationHitCounts => MutationHitCounts;

    private CoverageRunResult(string testId, CoverageConfidence confidence, IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations, IEnumerable<int> leakedMutations,
        IReadOnlyDictionary<int, int>? mutationHitCounts)
    {
        TestId = testId;

        foreach (var coveredMutation in coveredMutations)
        {
            MutationFlags[coveredMutation] = MutationTestingRequirements.None;
        }

        foreach (var detectedStaticMutation in detectedStaticMutations)
        {
            MutationFlags[detectedStaticMutation] = MutationTestingRequirements.Static;
        }

        foreach (var leakedMutation in leakedMutations)
        {
            var requirement = confidence == CoverageConfidence.Exact ?
                MutationTestingRequirements.NeedEarlyActivation :
                MutationTestingRequirements.CoveredOutsideTest;

            MutationFlags[leakedMutation] = requirement;
        }

        if (mutationHitCounts is not null)
        {
            foreach (var (mutantId, hitCount) in mutationHitCounts)
            {
                MutationHitCounts[mutantId] = hitCount;
            }
        }

        Confidence = confidence;
    }

    public static CoverageRunResult Create(
        string testId,
        CoverageConfidence confidence,
        IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations,
        IEnumerable<int> leakedMutations,
        IReadOnlyDictionary<int, int>? mutationHitCounts = null) =>
        new(testId, confidence, coveredMutations, detectedStaticMutations, leakedMutations, mutationHitCounts);

    public MutationTestingRequirements this[int mutation] => MutationFlags.TryGetValue(mutation, out var value) ? value : MutationTestingRequirements.NotCovered;

    public string TestId { get; }

    public IReadOnlyCollection<int> MutationsCovered => MutationFlags.Keys;

    public CoverageConfidence Confidence { get; private set; }

    public void Merge(ICoverageRunResult coverageRunResult)
    {
        var coverage = (CoverageRunResult)coverageRunResult;
        Confidence = (CoverageConfidence)Math.Min((int)Confidence, (int)coverage.Confidence);
        foreach (var mutationFlag in coverage.MutationFlags)
        {
            if (MutationFlags.ContainsKey(mutationFlag.Key))
            {
                MutationFlags[mutationFlag.Key] |= mutationFlag.Value;
            }
            else
            {
                MutationFlags[mutationFlag.Key] = mutationFlag.Value;
            }
        }

        foreach (var (mutantId, hitCount) in coverage.MutationHitCounts)
        {
            MutationHitCounts[mutantId] = MutationHitCounts.TryGetValue(mutantId, out var existingHitCount)
                ? existingHitCount + hitCount
                : hitCount;
        }
    }
}
