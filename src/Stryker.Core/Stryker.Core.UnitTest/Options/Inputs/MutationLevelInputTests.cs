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
            var target = new MutationLevelInput();
            target.SuppliedInput = "gibberish";

            var ex = Should.Throw<StrykerInputException>(() =>
            {
                target.Validate();
            });
            ex.Message.ShouldBe($"The given mutation level (gibberish) is invalid. Valid options are: [Basic, Standard, Advanced, Complete]");
        }
    }
}
