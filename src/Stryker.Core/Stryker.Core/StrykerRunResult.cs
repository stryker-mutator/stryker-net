using Stryker.Core.Options;

namespace Stryker.Core
{
    public class StrykerRunResult
    {
        private StrykerOptions _options { get; }
        public decimal? MutationScore { get; private set; }

        public StrykerRunResult(StrykerOptions options, decimal? mutationScore)
        {
            _options = options;
            MutationScore = mutationScore;
        }

        public bool IsScoreAboveThresholdBreak()
        {
            if (MutationScore == null)
            {
                // Return true, because there were no mutations created.
                return true;
            }

            // Check if the mutation score is not below the threshold break
            return MutationScore >= _options.Thresholds.Break;
        }
    }
}
