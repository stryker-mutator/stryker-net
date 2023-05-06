using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class FallbackVersionInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new FallbackVersionInput();
        target.HelpText.ShouldBe(@"Commitish used as a fallback when no report could be found based on Git information for the baseline feature.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
When you don't specify a fallback version the since target will be used as fallback version.
Example: If the current branch is based on the main branch, set 'main' as the fallback version | default: 'master'");
    }

    [Fact]
    public void ShouldNotValidate_IfNotEnabled()
    {
        var input = new FallbackVersionInput { SuppliedInput = "master" };

        var validatedInput = input.Validate(withBaseline: false, projectVersion: "master", sinceTarget: "master");

        validatedInput.ShouldBe(new SinceTargetInput().Default);
    }

    [Fact]
    public void ShouldUseProvidedInputValue()
    {
        var input = new FallbackVersionInput { SuppliedInput = "development" };

        var validatedInput = input.Validate(withBaseline: true, projectVersion: "feat/feat4", sinceTarget: "master");

        validatedInput.ShouldBe("development");
    }

    [Fact]
    public void ShouldUseSinceTarget_IfNotExplicitlySet()
    {
        var input = new FallbackVersionInput();

        var validatedInput = input.Validate(withBaseline: true, projectVersion: "development", sinceTarget: "main");

        validatedInput.ShouldBe("main");
    }
}
