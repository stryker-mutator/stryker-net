using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class MutationLevelInputTests
    {
        [Fact]
        public void ShouldValidateMutationLevel()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new MutationLevelInput { SuppliedInput = "gibberish" }.Validate();
            });
            ex.Details.ShouldBe($"The given mutation level (gibberish) is invalid. Valid options are: [Basic, Standard, Advanced, Complete]");
        }
    }
}
