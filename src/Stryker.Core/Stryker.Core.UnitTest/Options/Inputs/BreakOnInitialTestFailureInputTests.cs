using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class BreakOnInitialTestFailureInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new BreakOnInitialTestFailureInput();
        target.HelpText.ShouldBe(@"Instruct Stryker to break execution when at least one test failed on initial run. | default: 'False'");
    }
    
    [Theory]
    [InlineData(null, false)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void ShouldTranslateInputToExpectedResult(bool? argValue, bool expected)
    {
        var validatedInput = new BreakOnInitialTestFailureInput { SuppliedInput = argValue }.Validate();

        validatedInput.ShouldBe(expected);
    }
}
