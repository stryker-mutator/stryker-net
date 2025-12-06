using Stryker.Abstractions.Options;

namespace Stryker.Core;

public class StrykerRunResult
{
    public IStrykerOptions Options { get; }
    public double MutationScore { get; private set; }

    public StrykerRunResult(IStrykerOptions options, double mutationScore)
    {
        Options = options;
        MutationScore = mutationScore;
    }

    public bool ScoreIsLowerThanThresholdBreak()
    {
        // If the mutation score is NaN we don't have a result yet
        return !double.IsNaN(MutationScore) && MutationScore < (double)Options.Thresholds.Break / 100;
    }
}
