using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class TestsFailureBreakInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new TestsFailureBreakInput();
            target.HelpText.ShouldBe(@"Instruct Stryker to break execution when at least one test failed on initial run. | default: 'False'");
        }
        
        [Theory]
        [InlineData(null, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void ShouldTranslateInputToTestFailureBreakBehavior(bool? argValue, bool expected)
        {
            var validatedInput = new TestsFailureBreakInput { SuppliedInput = argValue }.Validate();

            validatedInput.ShouldBe(expected);
        }
    }
}
