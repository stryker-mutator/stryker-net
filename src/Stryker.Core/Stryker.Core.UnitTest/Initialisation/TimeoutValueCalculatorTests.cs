using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Initialisation;

namespace Stryker.Core.UnitTest.Initialisation;

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

    [TestMethod]
    [DataRow(1000, 0, 2.0, 2000)]
    [DataRow(1000, 500, 2.0, 2500)]
    [DataRow(1000, 0, 1.0, 1000)]
    [DataRow(2000, 1000, 3.0, 7000)]
    public void Calculator_ShouldUseConfiguredRatio(int baseTime, int extra, double ratio, int expected)
    {
        var target = new TimeoutValueCalculator(extra, ratio);

        target.CalculateTimeoutValue(baseTime).ShouldBe(expected);
    }
}
