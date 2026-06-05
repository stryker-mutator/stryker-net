using Shouldly;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;
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
    public void ShouldThrow_WhenDiskBaselineAndNotSupplied(string input)
    {
        var target = new BaselineOutputInput { SuppliedInput = input };

        Should.Throw<InputException>(() => target.Validate(BaselineProvider.Disk, withBaseline: true));
    }

    [TestMethod]
    public void ShouldReturnSuppliedValue_WhenDiskBaseline()
    {
        var target = new BaselineOutputInput { SuppliedInput = "custom-baseline" };

        var result = target.Validate(BaselineProvider.Disk, withBaseline: true);

        result.ShouldBe("custom-baseline");
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenBaselineDisabled()
    {
        var target = new BaselineOutputInput { SuppliedInput = null };

        var result = target.Validate(BaselineProvider.Disk, withBaseline: false);

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnDefault_WhenNonDiskProvider()
    {
        var target = new BaselineOutputInput { SuppliedInput = null };

        var result = target.Validate(BaselineProvider.Dashboard, withBaseline: true);

        result.ShouldBe(string.Empty);
    }
}
