using System;
using System.Collections.Generic;

namespace Stryker.Core.TestRunners;

public enum CoverageConfidence
{
    Exact,
    Normal,
    Dubious,
    UnexpectedCase
}

[Flags]
public enum MutationTestingRequirements
{
    None = 0,
    // mutation is static or executed inside à static context
    Static = 1,
    // mutation is covered outside test (before or after)
    CoveredOutsideTest = 2,
    // mutation needs to be activated ASAP when tested
    NeedEarlyActivation = 4,
    // mutation needs to be run in 'all tests' mode
    AgainstAllTests = 8,
    // not covered
    NotCovered = 256
}

public class CoverageRunResult
{
    private readonly Dictionary<int, MutationTestingRequirements> _mutationFlags = new();

    public Guid TestId { get; }

    public IReadOnlyCollection<int> MutationsCovered => _mutationFlags.Keys;

    public MutationTestingRequirements this[int mutation] => _mutationFlags.ContainsKey(mutation)
        ? _mutationFlags[mutation]
        : MutationTestingRequirements.NotCovered;

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

    public void Merge(CoverageRunResult coverageRunResult)
    {
        Confidence = (CoverageConfidence)Math.Min((int)Confidence, (int)coverageRunResult.Confidence);
        foreach (var mutationFlag in coverageRunResult._mutationFlags)
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
