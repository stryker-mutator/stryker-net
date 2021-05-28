using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class MutationLevelInputTests
    {
        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new MutationLevelInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBe(MutationLevel.Standard);
        }

        [Fact]
        public void ShouldThrowOnInvalidMutationLevel()
        {
            var target = new MutationLevelInput { SuppliedInput = "gibberish" };

            var ex = Should.Throw<InputException>(() => target.Validate());

            ex.Message.ShouldBe($"The given mutation level (gibberish) is invalid. Valid options are: [Basic, Standard, Advanced, Complete]");
        }

        [Theory]
        [InlineData("complete")]
        [InlineData("Complete")]
        public void ShouldReturnMutationLevel(string input)
        {
            var target = new MutationLevelInput { SuppliedInput = input };

            var result = target.Validate();

            result.ShouldBe(MutationLevel.Complete);
        }
    }
}
