using Stryker.Abstractions.Options;

namespace Stryker.Core
{
    public class StrykerRunResult
    {
        private readonly IStrykerOptions _options;
        public double MutationScore { get; private set; }

        public StrykerRunResult(IStrykerOptions options, double mutationScore)
        {
            _options = options;
            MutationScore = mutationScore;
        }

        public bool ScoreIsLowerThanThresholdBreak()
        {
            // If the mutation score is NaN we don't have a result yet
            return !double.IsNaN(MutationScore) && MutationScore < (double)_options.Thresholds.Break / 100;
        }
    }
}
