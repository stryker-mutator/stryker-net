using Shouldly;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest;

public class StrykerRunResultTests : TestBase
{
    [Theory]
    [InlineData(1, 80)]
    [InlineData(0.5, 50)]
    [InlineData(0.1, 0)]
    public void ScoreIsLowerThanThresholdBreak_ShouldReturnFalseWhen(double mutationScore, int thresholdBreak)
    {
        // Arrange
        var options = new StrykerOptions()
        {
            Thresholds = new Thresholds
            {
                High = 100,
                Low = 100,
                Break = thresholdBreak
            }
        };
        var runResult = new StrykerRunResult(options, mutationScore);

        // Act
        var scoreIsLowerThanThresholdBreak = runResult.ScoreIsLowerThanThresholdBreak();

        // Assert
        scoreIsLowerThanThresholdBreak.ShouldBeFalse("because the mutation score is higher than or equal to the threshold break");
    }

    [Theory]
    [InlineData(0.79, 80)]
    [InlineData(0.4, 50)]
    [InlineData(0, 1)]
    public void ScoreIsLowerThanThresholdBreak_ShouldReturnTrueWhen(double mutationScore, int thresholdBreak)
    {
        // Arrange
        var options = new StrykerOptions()
        {
            Thresholds = new Thresholds
            {
                High = 100,
                Low = 100,
                Break = thresholdBreak
            }
        };
        var runResult = new StrykerRunResult(options, mutationScore);

        // Act
        var scoreIsLowerThanThresholdBreak = runResult.ScoreIsLowerThanThresholdBreak();

        // Assert
        scoreIsLowerThanThresholdBreak.ShouldBeTrue("because the mutation score is lower than the threshold break");
    }
}
