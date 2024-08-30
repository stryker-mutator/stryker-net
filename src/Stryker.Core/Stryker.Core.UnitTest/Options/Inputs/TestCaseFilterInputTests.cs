using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Abstractions.UnitTest.Options.Inputs
{
    [TestClass]
    public class TestCaseFilterInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var input = new TestCaseFilterInput();
            input.HelpText.ShouldBe(@"Filters out tests in the project using the given expression.
Uses the syntax for dotnet test --filter option and vstest.console.exe --testcasefilter option.
For more information on running selective tests, see https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests. | default: ''");
        }

        [TestMethod]
        public void DefaultShouldBeEmpty()
        {
            var input = new TestCaseFilterInput();
            input.Default.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldReturnSuppliedInputWhenNotNullOrWhiteSpace()
        {
            var input = new TestCaseFilterInput { SuppliedInput = "Category=Unit" };
            input.Validate().ShouldBe("Category=Unit");
        }

        [TestMethod]
        public void ShouldReturnDefaultWhenSuppliedInputNull()
        {
            var input = new TestCaseFilterInput { SuppliedInput = null };
            input.Validate().ShouldBe("");
        }

        [TestMethod]
        public void ShouldReturnDefaultWhenSuppliedInputWhiteSpace()
        {
            var input = new TestCaseFilterInput { SuppliedInput = "    " };
            input.Validate().ShouldBe("");
        }
    }
}
