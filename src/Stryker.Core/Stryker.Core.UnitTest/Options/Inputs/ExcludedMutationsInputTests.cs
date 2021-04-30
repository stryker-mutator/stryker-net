using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ExcludedMutationsInputTests
    {
        [Fact]
        public void ShouldValidateExcludedMutation()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new ExcludedMutationsInput { SuppliedInput = new[] { "gibberish" } }.Validate();
            });
            ex.Details.ShouldBe($"Invalid excluded mutation (gibberish). The excluded mutations options are [Arithmetic, Equality, Boolean, Logical, Assignment, Unary, Update, Checked, Linq, String, Bitwise, Initializer, Regex]");
        }
    }
}
