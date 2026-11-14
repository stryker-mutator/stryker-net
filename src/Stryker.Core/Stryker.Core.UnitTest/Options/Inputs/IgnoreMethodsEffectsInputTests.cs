using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class IgnoreMethodsEffectsInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new IgnoreMethodsEffectsInput();
        target.HelpText.ShouldBe("Also ignore mutations whose only effect is on whether, or with what arguments, an ignored method is called. | default: 'False'");
    }

    [TestMethod]
    [DataRow(null, false)]
    [DataRow(false, false)]
    [DataRow(true, true)]
    public void ShouldTranslateInputToExpectedResult(bool? argValue, bool expected)
    {
        var validatedInput = new IgnoreMethodsEffectsInput { SuppliedInput = argValue }.Validate();

        validatedInput.ShouldBe(expected);
    }
}
