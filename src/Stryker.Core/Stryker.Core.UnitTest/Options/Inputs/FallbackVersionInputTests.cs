using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class FallbackVersionInputTests
    {
        [Fact]
        public void FallbackVersionCannotBeProjectVersion()
        {
            var input = new FallbackVersionInput { SuppliedInput = "master" };
            void act() => input.Validate("master");

            Should.Throw<StrykerInputException>(act)
                .Message.ShouldBe("Fallback version cannot be set to the same value as the dashboard-version, please provide a different fallback version");
        }

        [Fact]
        public void ShouldSetFallbackToBranchWhenNull()
        {
            var input = new FallbackVersionInput { SuppliedInput = null };

            var validatedInput = input.Validate("development");

            validatedInput.ShouldBe("development");
        }
    }
}
