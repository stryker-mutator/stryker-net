using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Stryker.Shared.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class DisableBailInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new DisableBailInput();
        target.HelpText.ShouldBe(@"Disable abort unit testrun as soon as the first unit test fails. | default: 'False'");
    }

    [Theory]
    [InlineData(false, OptimizationModes.None)]
    [InlineData(true, OptimizationModes.DisableBail)]
    [InlineData(null, OptimizationModes.None)]
    public void ShouldValidate(bool? input, OptimizationModes expected)
    {
        var target = new DisableBailInput { SuppliedInput = input };

        var result = target.Validate();

        result.ShouldBe(expected);
    }
}
