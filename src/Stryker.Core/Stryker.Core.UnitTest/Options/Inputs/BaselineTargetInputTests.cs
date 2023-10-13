using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class BaselineTargetInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineTargetInput();
        target.HelpText.ShouldBe(
            "The target branch/commit to compare with the current codebase when the baseline feature is enabled. | default: 'master'");
    }

    [Fact]
    public void ShouldUseSuppliedInputWhenSinceEnabled()
    {
        var suppliedInput = "develop";
        var validatedBaselineBranch =
            new BaselineTargetInput { SuppliedInput = suppliedInput }.Validate(sinceEnabled: true);
        validatedBaselineBranch.ShouldBe(suppliedInput);
    }

    [Fact]
    public void ShouldUseDefaultWhenSinceEnabledAndInputNull()
    {
        var input = new BaselineTargetInput();
        var validatedBaselineBranch = input.Validate(sinceEnabled: true);
        validatedBaselineBranch.ShouldBe(input.Default);
    }

    [Fact]
    public void MustNotBeEmptyStringWhenSinceEnabled()
    {
        var ex = Assert.Throws<InputException>(() =>
        {
            new BaselineTargetInput { SuppliedInput = "" }.Validate(sinceEnabled: true);
        });
        ex.Message.ShouldBe("The baseline target cannot be empty when the since feature is enabled");
    }

    [Fact]
    public void ShouldNotValidateBaselineTargetWhenSinceDisabled()
    {
        var input = new BaselineTargetInput { SuppliedInput = "develop" };
        var validatedBaselineBranch = input.Validate(sinceEnabled: false);
        validatedBaselineBranch.ShouldBe(input.Default);
    }
}
