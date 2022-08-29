using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class TestFailureBreakBehaviorInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new TestFailureBreakBehaviorInput();
            target.HelpText.ShouldBe(@"Initial test run failure behavior. | default: 'when-half' | allowed: never, when-any, when-half");
        }

        [Fact]
        public void ShouldBeWhenHalfWhenNull()
        {
            var input = new TestFailureBreakBehaviorInput { SuppliedInput = null };
            var validatedInput = input.Validate();

            validatedInput.ShouldBe(TestFailureBreakBehavior.WhenHalf);
        }

        [Theory]
        [InlineData("never", TestFailureBreakBehavior.Never)]
        [InlineData("when-any", TestFailureBreakBehavior.WhenAny)]
        [InlineData("when-half", TestFailureBreakBehavior.WhenHalf)]
        public void ShouldTranslateInputToTestFailureBreakBehavior(string argValue, TestFailureBreakBehavior expectedBehavior)
        {
            var validatedInput = new TestFailureBreakBehaviorInput { SuppliedInput = argValue }.Validate();

            validatedInput.ShouldBe(expectedBehavior);
        }

        [Theory]
        [InlineData("incorrect")]
        [InlineData("")]
        public void ShouldThrowWhenInputCannotBeTranslated(string argValue)
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                new TestFailureBreakBehaviorInput { SuppliedInput = argValue }.Validate();
            });

            ex.Message.ShouldBe($"Incorrect initial test run behavior ({argValue}). The behaviors are [Never, When-Any, When-Half]");
        }
    }
}
