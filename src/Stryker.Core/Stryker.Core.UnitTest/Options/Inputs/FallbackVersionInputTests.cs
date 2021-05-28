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

            var exception = Should.Throw<InputException>(() => input.Validate("master", true));

            exception.Message.ShouldBe("Fallback version cannot be set to the same value as the dashboard-version, please provide a different fallback version");
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
