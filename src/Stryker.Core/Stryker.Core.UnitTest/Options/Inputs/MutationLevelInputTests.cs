using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class MutationLevelInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new MutationLevelInput();
            target.HelpText.ShouldBe(@"Specify which mutation levels to place. Every higher level includes the mutations from the lower levels. | default: 'Standard' | allowed: Basic, Standard, Advanced, Complete");
        }

        [TestMethod]
        public void ShouldHaveDefault()
        {
            var target = new MutationLevelInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        public void ShouldThrowOnInvalidMutationLevel()
        {
            var target = new MutationLevelInput { SuppliedInput = "gibberish" };

            var ex = Should.Throw<InputException>(() => target.Validate());

            ex.Message.ShouldBe($"The given mutation level (gibberish) is invalid. Valid options are: [Basic, Standard, Advanced, Complete]");
        }

        [TestMethod]
        [DataRow("complete")]
        [DataRow("Complete")]
        public void ShouldReturnMutationLevel(string input)
        {
            var target = new MutationLevelInput { SuppliedInput = input };

            var result = target.Validate();

            result.ShouldBe(MutationLevel.Complete);
        }
    }
}
