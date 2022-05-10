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
        Dubious
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

        public CoverageRunResult Merge(CoverageRunResult other)
        {
            CoverageConfidence confidence;
            switch (Confidence)
            {
                case CoverageConfidence.Dubious:
                    confidence = Confidence;
                    break;
                case CoverageConfidence.Normal:
                    confidence = other.Confidence == CoverageConfidence.Dubious ? other.Confidence : Confidence;
                    break;
                default:
                    confidence = other.Confidence;
                    break;

            }

            return new CoverageRunResult(TestId,
                confidence,
                CoveredMutations.Union(other.CoveredMutations),
                DetectedStaticMutations.Union(other.CoveredMutations),
                LeakedMutations.Union(other.LeakedMutations));
        }
    }
}
