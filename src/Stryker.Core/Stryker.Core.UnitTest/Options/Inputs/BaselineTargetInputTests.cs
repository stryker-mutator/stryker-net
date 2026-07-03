using Shouldly;
using Stryker.Abstractions.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class BaselineTargetInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineTargetInput();
        target.HelpText.ShouldBe(
            "The target branch/commit to compare with the current codebase when the baseline feature is enabled. | default: 'master'");
    }

    [TestMethod]
    public void ShouldUseSuppliedInputWhenSinceEnabled()
    {
        var suppliedInput = "develop";
        var validatedBaselineBranch =
            new BaselineTargetInput { SuppliedInput = suppliedInput }.Validate(sinceEnabled: true);
        validatedBaselineBranch.ShouldBe(suppliedInput);
    }

    [TestMethod]
    public void ShouldUseDefaultWhenSinceEnabledAndInputNull()
    {
        var input = new BaselineTargetInput();
        var validatedBaselineBranch = input.Validate(sinceEnabled: true);
        validatedBaselineBranch.ShouldBe(input.Default);
    }

    [TestMethod]
    public void MustNotBeEmptyStringWhenSinceEnabled()
    {
        Should.Throw<InputException>(() =>
        {
            new BaselineTargetInput { SuppliedInput = "" }.Validate(sinceEnabled: true);
        }).Message.ShouldBe("The baseline target cannot be empty when the since feature is enabled");
    }

    [TestMethod]
    public void ShouldNotValidateBaselineTargetWhenSinceDisabled()
    {
        var input = new BaselineTargetInput { SuppliedInput = "develop" };
        var validatedBaselineBranch = input.Validate(sinceEnabled: false);
        validatedBaselineBranch.ShouldBe(input.Default);
    }
}
