using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class BaselineCompareVersionInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineCompareVersionInput();

        target.HelpText.ShouldBe("The version of the baseline report to compare the current codebase with. When empty the version of the current run is used, resulting in an incremental run. | default: ''");
    }

    [TestMethod]
    public void ShouldUseSuppliedInputWhenBaselineEnabled()
    {
        var suppliedInput = "develop";

        var validatedVersion = new BaselineCompareVersionInput { SuppliedInput = suppliedInput }.Validate(baselineEnabled: true);

        validatedVersion.ShouldBe(suppliedInput);
    }

    [TestMethod]
    public void ShouldBeEmptyWhenBaselineEnabledAndInputNull()
    {
        var validatedVersion = new BaselineCompareVersionInput().Validate(baselineEnabled: true);

        validatedVersion.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldAllowEmptyStringWhenBaselineEnabled()
    {
        var validatedVersion = new BaselineCompareVersionInput { SuppliedInput = "" }.Validate(baselineEnabled: true);

        validatedVersion.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldNotValidateCompareVersionWhenBaselineDisabled()
    {
        var validatedVersion = new BaselineCompareVersionInput { SuppliedInput = "develop" }.Validate(baselineEnabled: false);

        validatedVersion.ShouldBe(string.Empty);
    }
}
