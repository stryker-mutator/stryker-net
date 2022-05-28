using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Stryker.Core.TestRunners
{
    public enum CoverageConfidence
    {
        Exact,
        Normal,
        Dubious,
        UnexpectedCase
    }

    public class CoverageRunResult
    {
        public Guid TestId { get; }

        public CoverageConfidence Confidence { get; }

        public IReadOnlyCollection<int> CoveredMutations { get; }

        public IReadOnlyCollection<int> DetectedStaticMutations { get; }

        public IReadOnlyCollection<int> LeakedMutations { get; }

        public CoverageRunResult(Guid testId, CoverageConfidence confidence, IEnumerable<int> coveredMutations,
            IEnumerable<int> detectedStaticMutations, IEnumerable<int> leakedMutations)
        {
            TestId = testId;
            Confidence = confidence;
            CoveredMutations = coveredMutations?.ToImmutableArray();
            DetectedStaticMutations = detectedStaticMutations?.ToImmutableArray();
            LeakedMutations = leakedMutations?.ToImmutableArray();
        }
    }
}
