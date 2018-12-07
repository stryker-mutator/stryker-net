using Stryker.Core.Options;

namespace Stryker.Core
{
    public class StrykerRunResult {
        private StrykerOptions _options { get; }
        public decimal? MutationScore { get; }

        public StrykerRunResult(StrykerOptions options, decimal? mutationScore) 
        {
            _options = options;
            MutationScore = mutationScore;
        }

        public bool IsScoreAboveThresholdBreak() {
            decimal thresholdBreak = (decimal) _options.ThresholdOptions.ThresholdBreak / 100;
            // Check if the mutation score is not below the threshold break
            return MutationScore >= thresholdBreak;
        }
    } 
}
