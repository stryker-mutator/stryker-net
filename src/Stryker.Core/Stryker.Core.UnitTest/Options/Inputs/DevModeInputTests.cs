using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class DevModeInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new DevModeInput();
        target.HelpText.ShouldBe(@"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
    Setting this flag makes stryker not remove the mutations but rather crash on failed rollbacks | default: 'False'");
    }

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
