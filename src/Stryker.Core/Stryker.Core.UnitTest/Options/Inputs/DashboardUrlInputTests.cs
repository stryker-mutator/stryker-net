using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class DashboardUrlInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new DashboardUrlInput();
            target.HelpText.ShouldBe(@"Alternative url for Stryker Dashboard. | default: 'https://dashboard.stryker-mutator.io'");
        }

        [TestMethod]
        public void ShouldHaveDefault()
        {
            var target = new DashboardUrlInput { SuppliedInput = null };

            var defaultValue = target.Validate();

            defaultValue.ShouldBe("https://dashboard.stryker-mutator.io");
        }

        [TestMethod]
        public void ShouldAllowValidUri()
        {
            var target = new DashboardUrlInput { SuppliedInput = "http://example.com:8042" };

            var defaultValue = target.Validate();

            defaultValue.ShouldBe("http://example.com:8042");
        }

        [TestMethod]
        public void ShouldThrowOnInvalidUri()
        {
            var target = new DashboardUrlInput { SuppliedInput = "test" };

            var exception = Should.Throw<InputException>(() => target.Validate());

            exception.Message.ShouldBe("Stryker dashboard url 'test' is invalid.");
        }
    }
}
