using Shouldly;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class BreakOnInitialTestFailureInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new BreakOnInitialTestFailureInput();
            target.HelpText.ShouldBe(@"Instruct Stryker to break execution when at least one test failed on initial run. | default: 'False'");
        }
        
        [TestMethod]
        [DataRow(null, false)]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void ShouldTranslateInputToExpectedResult(bool? argValue, bool expected)
        {
            var validatedInput = new BreakOnInitialTestFailureInput { SuppliedInput = argValue }.Validate();

            validatedInput.ShouldBe(expected);
        }
    }
}
