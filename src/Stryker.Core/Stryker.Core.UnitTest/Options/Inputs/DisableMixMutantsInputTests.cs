using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DisableMixMutantsInputTests
    {
        [Theory]
        [InlineData(false, OptimizationModes.NoOptimization)]
        [InlineData(true, OptimizationModes.DisableMixMutants)]
        [InlineData(null, OptimizationModes.NoOptimization)]
        public void ShouldValidate(bool? input, OptimizationModes expected)
        {
            var target = new DisableMixMutantsInput { SuppliedInput = input };

            var result = target.Validate();

            result.ShouldBe(expected);
        }
    }
}
