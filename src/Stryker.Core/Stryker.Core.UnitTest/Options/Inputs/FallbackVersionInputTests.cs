using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class FallbackVersionInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new FallbackVersionInput();
        target.HelpText.ShouldBe(@"Version used as a bootstrap when no baseline report could be found for the compare version.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
Example: If the current branch is based on the main branch, set 'main' as the fallback version | default: 'main'");
    }

    [TestMethod]
    public void ShouldNotValidate_IfNotEnabled()
    {
        var input = new FallbackVersionInput { SuppliedInput = "development" };

        var validatedInput = input.Validate(withBaseline: false);

        validatedInput.ShouldBe("main");
    }

    [TestMethod]
    public void ShouldUseProvidedInputValue()
    {
        var input = new FallbackVersionInput { SuppliedInput = "development" };

        var validatedInput = input.Validate(withBaseline: true);

        validatedInput.ShouldBe("development");
    }

    [TestMethod]
    public void ShouldUseDefault_IfNotExplicitlySet()
    {
        var input = new FallbackVersionInput();

        var validatedInput = input.Validate(withBaseline: true);

        validatedInput.ShouldBe("main");
    }
}
