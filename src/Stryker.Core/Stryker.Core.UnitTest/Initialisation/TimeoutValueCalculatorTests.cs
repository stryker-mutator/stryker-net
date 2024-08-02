using Shouldly;
using Stryker.Configuration.Initialisation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Initialisation
{
    [TestClass]
    public class TimeoutValueCalculatorTests : TestBase
    {
        [TestMethod]
        [DataRow(1000, 0, 1500)]
        [DataRow(1000, 2000, 3500)]
        [DataRow(4000, 2000, 8000)]
        public void Calculator_ShouldCalculateTimeoutValueNoExtra(int baseTime, int extra, int expected)
        {
            var target = new TimeoutValueCalculator(extra);

            target.CalculateTimeoutValue(baseTime).ShouldBe(expected);
        }
    }
}
