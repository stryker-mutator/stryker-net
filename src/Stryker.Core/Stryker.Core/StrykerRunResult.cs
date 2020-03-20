using Stryker.Core.Options;

namespace Stryker.Core
{
    public class StrykerRunResult
    {
        private readonly StrykerOptions _options;
        public decimal? MutationScore { get; private set; }

        public StrykerRunResult(StrykerOptions options, decimal? mutationScore)
        {
            _options = options;
            MutationScore = mutationScore;
        }

        public bool ScoreIsLowerThanThresholdBreak()
        {
            // If the mutation score is null we don't have a result yet
            return MutationScore is null ? false : MutationScore < _options.Thresholds.Break;
        }
    }
}
