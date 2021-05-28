using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class SinceTargetInputTests
    {
        [Fact]
        public void ShouldUseDefaultWhenSinceEnabled()
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
            ex.Message.ShouldBe("The target branch/commit cannot be empty when the since feature is enabled");
        }

        [Fact]
        public void ShouldNotValidateSinceTargetWhenSinceDisabled()
        {
            var validatedSinceBranch = new SinceTargetInput { SuppliedInput = "develop" }.Validate(sinceEnabled: false);
            validatedSinceBranch.ShouldBe(null);
        }
    }
}
