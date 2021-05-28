using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ProjectUnderTestNameInputTests
    {
        [Fact]
        public void ShouldReturnName()
        {
            var target = new ProjectUnderTestNameInput { SuppliedInput = "name" };

            var result = target.Validate();

            result.ShouldBe("name");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new ProjectUnderTestNameInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBe("");
        }

        [Theory]
        [InlineData("")]
        public void ShouldThrowOnEmpty(string value)
        {
            var target = new ProjectUnderTestNameInput { SuppliedInput = value };

            var exception = Should.Throw<InputException>(() => target.Validate());

            exception.Message.ShouldBe("Project file cannot be empty.");
        }
    }
}
