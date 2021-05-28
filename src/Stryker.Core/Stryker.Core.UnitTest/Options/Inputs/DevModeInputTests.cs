using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DevModeInputTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(null, false)]
        public void ShouldValidate(bool? input, bool expected)
        {
            var target = new DevModeInput { SuppliedInput = input };

            var result = target.Validate();

            result.ShouldBe(expected);
        }
    }
}
