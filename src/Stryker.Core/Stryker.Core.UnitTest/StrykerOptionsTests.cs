using System;
using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class StrykerOptionsTests
    {
        [Theory]
        [InlineData("error", LogEventLevel.Error)]
        [InlineData("", LogEventLevel.Information)]
        [InlineData(null, LogEventLevel.Information)]
        [InlineData("warning", LogEventLevel.Warning)]
        [InlineData("info", LogEventLevel.Information)]
        [InlineData("debug", LogEventLevel.Debug)]
        [InlineData("trace", LogEventLevel.Verbose)]
        public void Constructor_WithCorrectLoglevelArgument_ShouldAssignCorrectLogLevel(string argValue, LogEventLevel expectedLogLevel)
        {
            var options = new StrykerOptions("c:/test", "Console", "", 0, argValue, false, 1, 80, 60, 0);

            options.LogOptions.ShouldNotBeNull();
            options.LogOptions.LogLevel.ShouldBe(expectedLogLevel);
        }

        [Fact]
        public void Constructor_WithIncorrectLoglevelArgument_ShouldThrowStrykerInputException()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions("c:/test", "Console", "", 0, logLevel, false, 1, 80, 60, 0);
            });

            ex.Message.ShouldBe("The value for one of your settings is not correct. Try correcting or removing them.");
        }

        [Fact]
        public void Constructor_WithIncorrectSettings_ShoudThrowWithDetails()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerOptions("c:/test", "Console", "", 0, logLevel, false, 1, 80, 60, 0);
            });

            ex.Details.ShouldNotBeNullOrEmpty();
        }
    }
}
