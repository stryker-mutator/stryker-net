using System;
using System.Collections.Generic;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.TestRunners.MsTest.Testing.Results;

internal class CoverageRunResult : ICoverageRunResult
{
    public Dictionary<int, MutationTestingRequirements> MutationFlags { get; } = new ();

    private CoverageRunResult(string testId, CoverageConfidence confidence, IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations, IEnumerable<int> leakedMutations)
    {
        TestId = Identifier.Create(testId);

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

        Confidence = confidence;
    }

    public static CoverageRunResult Create(
        string testId,
        CoverageConfidence confidence,
        IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations,
        IEnumerable<int> leakedMutations) => new(testId, confidence, coveredMutations, detectedStaticMutations, leakedMutations);

    public MutationTestingRequirements this[int mutation] => MutationFlags.TryGetValue(mutation, out var value) ? value : MutationTestingRequirements.NotCovered;

    public Identifier TestId { get; }

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
    }
}
