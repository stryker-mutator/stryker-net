using System;
using System.Collections.Generic;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.TestRunners;

public class CoverageRunResult : ICoverageRunResult
{
    private readonly Dictionary<int, MutationTestingRequirements> MutationFlags = new();

    public Identifier TestId { get; }

    public IReadOnlyCollection<int> MutationsCovered => MutationFlags.Keys;

    public MutationTestingRequirements this[int mutation] => MutationFlags.ContainsKey(mutation)
        ? MutationFlags[mutation]
        : MutationTestingRequirements.NotCovered;

    public CoverageConfidence Confidence { get; private set; }

    public CoverageRunResult(Identifier testId, CoverageConfidence confidence, IEnumerable<int> coveredMutations,
        IEnumerable<int> detectedStaticMutations, IEnumerable<int> leakedMutations)
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
            MutationFlags[leakedMutation] = confidence == CoverageConfidence.Exact ? MutationTestingRequirements.NeedEarlyActivation : MutationTestingRequirements.CoveredOutsideTest;
        }

        Confidence = confidence;
    }

    public void Merge(ICoverageRunResult coverageRunResult)
    {
        Confidence = (CoverageConfidence)Math.Min((int)Confidence, (int)coverageRunResult.Confidence);
        foreach (var mutationFlag in coverageRunResult.MutationFlags)
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
