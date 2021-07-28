using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class FallbackVersionInputTests
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new FallbackVersionInput();
            target.HelpText.ShouldBe(@"Commitish used as a fallback when no report could be found based on Git information for the baseline feature.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
When you don't specify a fallback version the since target will be used as fallback version.
Example: If the current branch is based on the main branch, set 'main' as the fallback version | default: ''");
        }

        [Fact]
        public void FallbackVersionCannotBeProjectVersion()
        {
            var input = new FallbackVersionInput { SuppliedInput = "master" };

            var exception = Should.Throw<InputException>(() => input.Validate("master", true));

            exception.Message.ShouldBe("Fallback version cannot be set to the same value as the current project version, please provide a different fallback version");
        }

        [Fact]
        public void ShouldAllowDevelopment()
        {
            var input = new FallbackVersionInput { SuppliedInput = "development" };

            var validatedInput = input.Validate("master", true);

            validatedInput.ShouldBe("development");
        }
    }
}
