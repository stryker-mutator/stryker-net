using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Stryker.Core.TestRunners
{
    public enum CoverageConfidence
    {
        Exact,
        Normal,
        Dubious,
        UnexpectedCase
    }

    [Flags]
    public enum MutationFlags
    {
        Normal = 0,
        Static = 1,
        CoveredOutsideTest = 2,
        NeedEarlyActivation = 4,
    }

    public class CoverageRunResult
    {
        private readonly Dictionary<int, MutationFlags> _mutationFlags = new();

        public Guid TestId { get; }

        public CoverageConfidence Confidence { get; private set; }

        public IReadOnlyCollection<int> CoveredMutations => _mutationFlags
            .Where(p => !p.Value.HasFlag(MutationFlags.CoveredOutsideTest)).Select(p => p.Key).ToImmutableArray();

        public IReadOnlyCollection<int> DetectedStaticMutations => _mutationFlags
            .Where(p => p.Value.HasFlag(MutationFlags.Static)).Select(p => p.Key).ToImmutableArray();

        public IReadOnlyCollection<int> LeakedMutations  => _mutationFlags
            .Where(p => p.Value.HasFlag(MutationFlags.CoveredOutsideTest)).Select(p => p.Key).ToImmutableArray();

        public IReadOnlyCollection<int> MutationsToTestInIsolation => _mutationFlags
            .Where(p => p.Value.HasFlag(MutationFlags.NeedEarlyActivation)).Select(p => p.Key).ToImmutableArray();

        public CoverageRunResult(Guid testId, CoverageConfidence confidence, IEnumerable<int> coveredMutations,
            IEnumerable<int> detectedStaticMutations, IEnumerable<int> leakedMutations)
        {
            TestId = testId;
            foreach (var coveredMutation in coveredMutations)
            {
                _mutationFlags[coveredMutation] = MutationFlags.Normal;
            }

            foreach (var detectedStaticMutation in detectedStaticMutations)
            {
                _mutationFlags[detectedStaticMutation] = MutationFlags.Static;
            }

            foreach (var leakedMutation in leakedMutations)
            {
                _mutationFlags[leakedMutation] = confidence == CoverageConfidence.Exact ? MutationFlags.NeedEarlyActivation: MutationFlags.CoveredOutsideTest;
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

        public void ConfirmCoverageForLeakedMutations(ISet<int> mutationSeenInSetup)
        {
            foreach (var i in mutationSeenInSetup)
            {
                if (!_mutationFlags.ContainsKey(i))
                {
                    _mutationFlags[i] = MutationFlags.NeedEarlyActivation;
                    continue;
                }
                _mutationFlags[i] = (_mutationFlags[i] & ~MutationFlags.CoveredOutsideTest) | MutationFlags.NeedEarlyActivation;
            }
        }
    }
}
