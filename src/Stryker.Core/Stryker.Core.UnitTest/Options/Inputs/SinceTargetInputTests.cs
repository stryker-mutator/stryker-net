using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class SinceTargetInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new SinceTargetInput();
            target.HelpText.ShouldBe(@"The target branch/commit to compare with the current codebase when the since feature is enabled. | default: 'master'");
        }

        [TestMethod]
        public void ShouldUseSuppliedInputWhenSinceEnabled()
        {
            var suppliedInput = "develop";
            var validatedSinceBranch = new SinceTargetInput { SuppliedInput = suppliedInput }.Validate(sinceEnabled: true);
            validatedSinceBranch.ShouldBe(suppliedInput);
        }

        [TestMethod]
        public void ShouldUseDefaultWhenSinceEnabledAndInputNull()
        {
            var input = new SinceTargetInput();
            var validatedSinceBranch = input.Validate(sinceEnabled: true);
            validatedSinceBranch.ShouldBe(input.Default);
        }

        [TestMethod]
        public void MustNotBeEmptyStringWhenSinceEnabled()
        {
            var ex = Should.Throw<InputException>(() =>
            {
                new SinceTargetInput { SuppliedInput = "" }.Validate(sinceEnabled: true);
            });
            ex.Message.ShouldBe("The since target cannot be empty when the since feature is enabled");
        }

        [TestMethod]
        public void ShouldNotValidateSinceTargetWhenSinceDisabled()
        {
            var validatedSinceBranch = new SinceTargetInput { SuppliedInput = "develop" }.Validate(sinceEnabled: false);
            validatedSinceBranch.ShouldBe("master");
        }
    }
}
