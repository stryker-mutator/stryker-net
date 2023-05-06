using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class BaselineProviderInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineProviderInput();
        target.HelpText.ShouldBe("Choose a storage location for dashboard compare. Set to Dashboard provider when the dashboard reporter is turned on. | default: 'disk' | allowed: Dashboard, Disk, AzureFileStorage");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new BaselineProviderInput { SuppliedInput = null };

        var result = target.Validate(new Reporter[] { });

        target.Default.ShouldBe("disk");
        result.ShouldBe(BaselineProvider.Disk);
    }

    [Fact]
    public void ShouldHaveDefaultForDashboard()
    {
        var target = new BaselineProviderInput { SuppliedInput = null };

        var result = target.Validate(new[] { Reporter.Dashboard });

        result.ShouldBe(BaselineProvider.Dashboard);
    }

    [Theory]
    [InlineData("disk")]
    [InlineData("Disk")]
    public void ShouldSetDisk(string value)
    {
        var target = new BaselineProviderInput { SuppliedInput = value };

        var result = target.Validate(new[] { Reporter.Dashboard });

        result.ShouldBe(BaselineProvider.Disk);
    }

    [Theory]
    [InlineData("dashboard")]
    [InlineData("Dashboard")]
    public void ShouldSetDashboard(string value)
    {
        var target = new BaselineProviderInput { SuppliedInput = value };

        var result = target.Validate(new[] { Reporter.Dashboard });

        result.ShouldBe(BaselineProvider.Dashboard);
    }

    [Theory]
    [InlineData("azurefilestorage")]
    [InlineData("AzureFileStorage")]
    public void ShouldSetAzureFileStorage(string value)
    {
        var target = new BaselineProviderInput { SuppliedInput = value };

        var result = target.Validate(new[] { Reporter.Dashboard });

        result.ShouldBe(BaselineProvider.AzureFileStorage);
    }

    [Fact]
    public void ShouldThrowException_OnInvalidInput()
    {
        var target = new BaselineProviderInput { SuppliedInput = "invalid" };

        var exception = Should.Throw<InputException>(() => target.Validate(new[] { Reporter.Dashboard }));

        exception.Message.ShouldBe("Baseline storage provider 'invalid' does not exist");
    }
}
