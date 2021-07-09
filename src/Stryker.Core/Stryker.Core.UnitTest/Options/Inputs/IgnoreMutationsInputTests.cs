using System.Linq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class IgnoreMutationsInputTests
    {
        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new IgnoreMutationsInput();
            target.HelpText.ShouldBe(@"The given mutators will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['string', 'logical'] | default: []");
        }

        [Fact]
        public void ShouldValidateExcludedMutation()
        {
            var target = new IgnoreMutationsInput { SuppliedInput = new[] { "gibberish" } };

            var ex = Should.Throw<InputException>(() => target.Validate());

            ex.Message.ShouldBe($"Invalid excluded mutation (gibberish). The excluded mutations options are [Arithmetic, Equality, Boolean, Logical, Assignment, Unary, Update, Checked, Linq, String, Statement, Bitwise, Initializer, Regex]");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new IgnoreMutationsInput { SuppliedInput = new string[] { } };

            var result = target.Validate();

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldReturnMultipleMutators()
        {
            var target = new IgnoreMutationsInput { SuppliedInput = new[] {
                Mutator.String.ToString(),
                Mutator.Logical.ToString()
            } };

            var result = target.Validate();

            result.Count().ShouldBe(2);
            result.First().ShouldBe(Mutator.String);
            result.Last().ShouldBe(Mutator.Logical);
        }
    }
}
