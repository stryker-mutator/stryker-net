using Shouldly;
using Stryker.Core.Initialisation;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class TimeoutValueCalculatorTests
    {
        [Fact]
        public void Calculator_ShouldCalculateTimeoutValueNoExtra()
        {
            var target = new TimeoutValueCalculator();

            var result = target.CalculateTimeoutValue(1000, 0);

            result.ShouldBe(1500);
        }

        [Fact]
        public void Calculator_ShouldCalculateTimeoutValue1000()
        {
            var target = new TimeoutValueCalculator();

            var result = target.CalculateTimeoutValue(1000, 2000);

            result.ShouldBe(3500);
        }

        [Fact]
        public void Calculator_ShouldCalculateTimeoutValue4000()
        {
            var target = new TimeoutValueCalculator();

            var result = target.CalculateTimeoutValue(4000, 2000);

            result.ShouldBe(8000);
        }
    }
}
