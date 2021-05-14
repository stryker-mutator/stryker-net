using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class VerbosityInputTests
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
            var validatedInput = new VerbosityInput { SuppliedInput = argValue }.Validate();

            validatedInput.ShouldBe(expectedLogLevel);
        }

        [Fact]
        public void ShouldValidateLoglevel()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new VerbosityInput { SuppliedInput = logLevel }.Validate();
            });

            ex.Message.ShouldBe($"Incorrect verbosity (incorrect). The verbosity options are [Trace, Debug, Info, Warning, Error]");
        }
    }
}
