using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class OpenReportEnabledInputTests : TestBase
{
    [Fact]
    public void ShouldHaveNoHelpText()
    {
        var target = new OpenReportEnabledInput();
        target.HelpText.ShouldBe(@" | default: 'False'");
    }

    [Fact]
    public void ShouldSetToTrue()
    {
        var target = new OpenReportEnabledInput { SuppliedInput = true };
        target.Validate().ShouldBeTrue();
    }

    [Fact]
    public void ShouldSetToFalse()
    {
        var target = new OpenReportEnabledInput { SuppliedInput = false };
        target.Validate().ShouldBeFalse();
    }
}
