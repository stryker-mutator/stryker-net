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
        [InlineData("", LogEventLevel.Warning)]
        [InlineData(null, LogEventLevel.Warning)]
        [InlineData("warning", LogEventLevel.Warning)]
        [InlineData("info", LogEventLevel.Information)]
        [InlineData("debug", LogEventLevel.Debug)]
        [InlineData("trace", LogEventLevel.Verbose)]
        public void Constructor_WithCorrectLoglevelArgument_ShouldAssignCorrectLogLevel(string argValue, LogEventLevel expectedLogLevel)
        {
            var options = new StrykerOptions("c:/test", "Console", "", 0, argValue, false);

            Assert.NotNull(options.LogOptions);
            Assert.Equal(expectedLogLevel, options.LogOptions.LogLevel);
        }

        [Fact]
        public void Constructor_WithIncorrectLoglevelArgument_ShouldThrowValidationException()
        {
            var logLevel = "incorrect";

            Func<StrykerOptions> createOptions = () => new StrykerOptions("c:/test", "Console", "", 0, logLevel, false);

            Assert.Throws<ValidationException>(createOptions);
        }
    }
}
