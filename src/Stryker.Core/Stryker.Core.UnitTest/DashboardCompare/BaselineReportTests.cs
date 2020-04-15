using Stryker.Core.DashboardCompare;
using System;
using System.Collections.Generic;
using System.Text;
using Shouldly;
using Xunit;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Options;

namespace Stryker.Core.UnitTest.DashboardCompare
{
    public class BaselineReportTests
    {
        [Fact]
        public void BaselineReport_Instance_Property_Returns_Fresh_Instance_When_Null()
        {
            // Act
            var instance = BaselineReport.Instance;

            // Assert
            instance.ShouldBeOfType<BaselineReport>();
        }

        [Fact]
        public void BaselineReport_Instance_Should_Return_Same_reference_When_Called_Twice()
        {
            // Arrange
            var instance1 = BaselineReport.Instance;

            var instance2 = BaselineReport.Instance;

            // Assert
            instance1.ShouldBe(instance2);
        }

        [Fact]
        public void BaselineReport_Setting_Report_Twice_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var instance = BaselineReport.Instance;

            instance.Report = JsonReport.Build(options, null);

            // Act
            void act() => instance.Report = JsonReport.Build(options, null);

            // Assert
            Should.Throw<InvalidOperationException>(act);
        }


    }
}
