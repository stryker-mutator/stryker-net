using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class VerbosityInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new VerbosityInput();
        target.HelpText.ShouldBe(@"The verbosity (loglevel) for output to the console. | default: 'info' | allowed: error, warning, info, debug, trace");
    }

    [Fact]
    public void ShouldBeInformationWhenNull()
    {
        var input = new VerbosityInput { SuppliedInput = null };
        var validatedInput = input.Validate();

        validatedInput.ShouldBe(LogEventLevel.Information);
    }

    [Theory]
    [InlineData("error", LogEventLevel.Error)]
    [InlineData("warning", LogEventLevel.Warning)]
    [InlineData("info", LogEventLevel.Information)]
    [InlineData("debug", LogEventLevel.Debug)]
    [InlineData("trace", LogEventLevel.Verbose)]
    public void ShouldTranslateLogLevelToLogEventLevel(string argValue, LogEventLevel expectedLogLevel)
    {
        var validatedInput = new VerbosityInput { SuppliedInput = argValue }.Validate();

        validatedInput.ShouldBe(expectedLogLevel);
    }

    [Theory]
    [InlineData("incorrect")]
    [InlineData("")]
    public void ShouldThrowWhenInputCannotBeTranslated(string logLevel)
    {
        var ex = Assert.Throws<InputException>(() =>
        {
            new VerbosityInput { SuppliedInput = logLevel }.Validate();
        });

        ex.Message.ShouldBe($"Incorrect verbosity ({logLevel}). The verbosity options are [Trace, Debug, Info, Warning, Error]");
    }
}
