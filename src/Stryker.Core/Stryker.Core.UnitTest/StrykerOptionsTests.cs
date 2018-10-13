using System;
using Serilog.Events;
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

            Assert.NotNull(options.LogOptions);
            Assert.Equal(expectedLogLevel, options.LogOptions.LogLevel);
        }

        [Fact]
        public void Constructor_WithIncorrectLoglevelArgument_ShouldThrowStrykerInputException()
        {
            var logLevel = "incorrect";

            Func<StrykerOptions> createOptions = () => new StrykerOptions("c:/test", "Console", "", 0, logLevel, false, 1, 80, 60, 0);

            Assert.Throws<StrykerInputException>(createOptions);
        }
    }
}
