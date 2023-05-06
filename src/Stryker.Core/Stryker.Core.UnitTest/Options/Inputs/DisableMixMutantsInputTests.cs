using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class DisableMixMutantsInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new DisableMixMutantsInput();
        target.HelpText.ShouldBe(@"Test each mutation in an isolated test run. | default: 'False'");
    }

    [Theory]
    [InlineData(false, OptimizationModes.None)]
    [InlineData(true, OptimizationModes.DisableMixMutants)]
    [InlineData(null, OptimizationModes.None)]
    public void ShouldValidate(bool? input, OptimizationModes expected)
    {
        var target = new DisableMixMutantsInput { SuppliedInput = input };

        var result = target.Validate();

        result.ShouldBe(expected);
    }
}
