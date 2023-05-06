using Stryker.Core.Options;

namespace Stryker.Core;

public class StrykerRunResult
{
    private readonly StrykerOptions _options;
    public double MutationScore { get; private set; }

    public StrykerRunResult(StrykerOptions options, double mutationScore)
    {
        _options = options;
        MutationScore = mutationScore;
    }

    public bool ScoreIsLowerThanThresholdBreak()
    {
        // If the mutation score is NaN we don't have a result yet
        return !double.IsNaN(MutationScore) && MutationScore < ((double)_options.Thresholds.Break / 100);
    }
}
