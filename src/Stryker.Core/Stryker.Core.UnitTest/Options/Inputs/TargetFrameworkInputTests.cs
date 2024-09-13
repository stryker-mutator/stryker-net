using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class TargetFrameworkInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new TargetFrameworkInput();
            target.HelpText.ShouldBe("The framework to build the project against.");
        }

        [TestMethod]
        public void ShouldHaveDefaultNull()
        {
            var target = new TargetFrameworkInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBeNull();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        public void ShouldThrowOnEmptyInput(string input)
        {
            var target = new TargetFrameworkInput { SuppliedInput = input };

            var exception = Should.Throw<InputException>(() => target.Validate());

            exception.Message.ShouldBe(
                "Target framework cannot be empty. " +
                "Please provide a valid value from this list: " +
                "https://docs.microsoft.com/en-us/dotnet/standard/frameworks");
        }

        [TestMethod]
        public void ShouldReturnFramework()
        {
            var target = new TargetFrameworkInput { SuppliedInput = "netcoreapp3.1" };

            var result = target.Validate();

            result.ShouldBe("netcoreapp3.1");
        }
    }
}
