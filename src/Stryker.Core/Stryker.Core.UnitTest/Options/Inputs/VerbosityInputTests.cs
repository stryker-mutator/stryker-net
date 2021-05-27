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

        [Theory]
        [InlineData("incorrect")]
        [InlineData("")]
        public void ShouldValidateLoglevel(string logLevel)
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                new VerbosityInput { SuppliedInput = logLevel }.Validate();
            });

            ex.Message.ShouldBe($"Incorrect verbosity ({logLevel}). The verbosity options are [Trace, Debug, Info, Warning, Error]");
        }
    }
}
