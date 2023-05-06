using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class SinceTargetInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new SinceTargetInput();
        target.HelpText.ShouldBe(@"The target branch/commit to compare with the current codebase when the since feature is enabled. | default: 'master'");
    }

    [Fact]
    public void ShouldUseSuppliedInputWhenSinceEnabled()
    {
        var suppliedInput = "develop";
        var validatedSinceBranch = new SinceTargetInput { SuppliedInput = suppliedInput }.Validate(sinceEnabled: true);
        validatedSinceBranch.ShouldBe(suppliedInput);
    }

    [Fact]
    public void ShouldUseDefaultWhenSinceEnabledAndInputNull()
    {
        var input = new SinceTargetInput();
        var validatedSinceBranch = input.Validate(sinceEnabled: true);
        validatedSinceBranch.ShouldBe(input.Default);
    }

    [Fact]
    public void MustNotBeEmptyStringWhenSinceEnabled()
    {
        var ex = Assert.Throws<InputException>(() =>
        {
            new SinceTargetInput { SuppliedInput = "" }.Validate(sinceEnabled: true);
        });
        ex.Message.ShouldBe("The since target cannot be empty when the since feature is enabled");
    }

    [Fact]
    public void ShouldNotValidateSinceTargetWhenSinceDisabled()
    {
        var validatedSinceBranch = new SinceTargetInput { SuppliedInput = "develop" }.Validate(sinceEnabled: false);
        validatedSinceBranch.ShouldBe("master");
    }
}
