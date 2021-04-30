using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class IgnoreMutationsInputTests
    {
        [Fact]
        public void ShouldValidateExcludedMutation()
        {
            var target = new IgnoreMutationsInput();
            target.SuppliedInput = new[] { "gibberish" };

            var ex = Should.Throw<StrykerInputException>(() =>
            {
                target.Validate();
            });
            ex.Message.ShouldBe($"Invalid excluded mutation (gibberish). The excluded mutations options are [Arithmetic, Equality, Boolean, Logical, Assignment, Unary, Update, Checked, Linq, String, Bitwise, Initializer, Regex]");
        }
    }
}
