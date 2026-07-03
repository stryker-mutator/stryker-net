using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class BaselineRecreateEnabledInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineRecreateEnabledInput();
        target.HelpText.ShouldBe("When enabled a new baseline will be created by doing a full run and storing the mutation results. | default: 'False'");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new BaselineRecreateEnabledInput { SuppliedInput = true };

        var result = target.Validate();

        target.Default.Value.ShouldBeFalse();
        result.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldHaveDefaultForDashboard()
    {
        var target = new BaselineRecreateEnabledInput { SuppliedInput = false };

        var result = target.Validate();

        target.Default.Value.ShouldBeFalse();
        result.ShouldBeFalse();
    }
}
