using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class DisableTimeoutsInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new DisableTimeoutsInput();
        target.HelpText.ShouldBe(@"After a mutation test run, adds Stryker disable comments to source files for mutants that caused timeouts. | default: 'False'");
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(true, true)]
    [DataRow(null, false)]
    public void ShouldValidate(bool? input, bool expected)
    {
        var target = new DisableTimeoutsInput { SuppliedInput = input };

        var result = target.Validate();

        result.ShouldBe(expected);
    }
}
