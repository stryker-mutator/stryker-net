using Shouldly;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class BaselineOutputInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineOutputInput();
        target.HelpText.ShouldNotBeNullOrEmpty();
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void ShouldReturnDefault_WhenNotSupplied(string input)
    {
        var target = new BaselineOutputInput { SuppliedInput = input };

        var result = target.Validate();

        result.ShouldBe("StrykerOutput");
    }

    [TestMethod]
    public void ShouldReturnSuppliedValue_WhenSupplied()
    {
        var target = new BaselineOutputInput { SuppliedInput = "custom-baseline" };

        var result = target.Validate();

        result.ShouldBe("custom-baseline");
    }
}
