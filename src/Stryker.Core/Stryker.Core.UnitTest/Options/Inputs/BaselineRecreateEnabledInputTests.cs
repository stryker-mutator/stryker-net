using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class BaselineRecreateEnabledInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineRecreateEnabledInput();
        target.HelpText.ShouldBe("When enabled a new baseline will be created by doing a full run and storing the mutation results. | default: 'False'");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new BaselineRecreateEnabledInput { SuppliedInput = true };

        var result = target.Validate();

        target.Default.ShouldBeFalse();
        result.ShouldBeTrue();
    }

    [Fact]
    public void ShouldHaveDefaultForDashboard()
    {
        var target = new BaselineRecreateEnabledInput { SuppliedInput = false };

        var result = target.Validate();

        target.Default.ShouldBeFalse();
        result.ShouldBeFalse();
    }
}
