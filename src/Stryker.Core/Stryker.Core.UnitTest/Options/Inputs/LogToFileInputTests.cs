using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class LogToFileInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new LogToFileInput();
        target.HelpText.ShouldBe(@"Makes the logger write to a file. Logging to file always uses loglevel trace. | default: 'False'");
    }

    [Fact]
    public void ShouldThrowIfTrueAndNoOutputPath()
    {
        var target = new LogToFileInput { SuppliedInput = true };

        var exception = Should.Throw<InputException>(() => target.Validate(null));

        exception.Message.ShouldBe("Output path must be set if log to file is enabled");
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(null, false)]
    public void ShouldValidate(bool? input, bool expected)
    {
        var target = new LogToFileInput { SuppliedInput = input };

        var result = target.Validate("TestPath");

        result.ShouldBe(expected);
    }
}
