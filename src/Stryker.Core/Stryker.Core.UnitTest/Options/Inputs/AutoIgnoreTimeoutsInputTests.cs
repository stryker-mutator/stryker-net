using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class AutoIgnoreTimeoutsInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new AutoIgnoreTimeoutsInput();
        target.HelpText.ShouldBe(@"Mark mutants that resulted in a timeout as ignored using a Stryker comment in the source code, so they are skipped in future runs. | default: 'False'");
    }

    [TestMethod]
    [DataRow(null, false)]
    [DataRow(false, false)]
    [DataRow(true, true)]
    public void ShouldTranslateInputToExpectedResult(bool? argValue, bool expected)
    {
        var validatedInput = new AutoIgnoreTimeoutsInput { SuppliedInput = argValue }.Validate();

        validatedInput.ShouldBe(expected);
    }
}
