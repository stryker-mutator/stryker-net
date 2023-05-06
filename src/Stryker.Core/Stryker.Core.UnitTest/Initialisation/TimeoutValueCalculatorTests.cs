using Shouldly;
using Stryker.Core.Initialisation;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation;

public class TimeoutValueCalculatorTests : TestBase
{
    [Theory]
    [InlineData(1000, 0, 1500)]
    [InlineData(1000, 2000, 3500)]
    [InlineData(4000, 2000, 8000)]
    public void Calculator_ShouldCalculateTimeoutValueNoExtra(int baseTime, int extra, int expected)
    {
        var target = new TimeoutValueCalculator(extra);

        target.CalculateTimeoutValue(baseTime).ShouldBe(expected);
    }
}
