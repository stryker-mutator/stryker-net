using System.Collections.Generic;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class BaselineProviderInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new BaselineProviderInput();
            target.HelpText.ShouldBe("Choose a storage location for dashboard compare. Set to Dashboard provider when the dashboard reporter is turned on. | default: 'disk' | allowed: Dashboard, Disk, AzureFileStorage");
        }

        [Fact]
        public void ShouldSetDefault_WhenBaselineIsDisabled()
        {
            var target = new BaselineProviderInput { SuppliedInput = "azurefilestorage" };

            var result = target.Validate(new Reporter[] { }, false);

            result.ShouldBe(BaselineProvider.Disk);
        }

        [Theory]
        [InlineData(new Reporter[] { })]
        [InlineData(new Reporter[] { Reporter.Baseline })]
        public void ShouldSetDefault_WhenInputIsNullAndDashboardReporterIsNotEnabled(IEnumerable<Reporter> reporters)
        {
            var target = new BaselineProviderInput { SuppliedInput = null };

            var result = target.Validate(reporters, true);

            result.ShouldBe(BaselineProvider.Disk);
        }

        [Theory]
        [InlineData(new Reporter[] { Reporter.Dashboard })]
        [InlineData(new Reporter[] { Reporter.Dashboard, Reporter.Baseline })]
        public void ShouldSetDashboard_WhenInputIsNullAndDashboardReporterIsEnabled(IEnumerable<Reporter> reporters)
        {
            var target = new BaselineProviderInput { SuppliedInput = null };

            var result = target.Validate(reporters, true);

            result.ShouldBe(BaselineProvider.Dashboard);
        }

        [Theory]
        [InlineData("disk")]
        [InlineData("Disk")]
        public void ShouldSetDisk(string value)
        {
            var target = new BaselineProviderInput { SuppliedInput = value };

            var result = target.Validate(new[] { Reporter.Dashboard }, true);

            result.ShouldBe(BaselineProvider.Disk);
        }

        [Theory]
        [InlineData("dashboard")]
        [InlineData("Dashboard")]
        public void ShouldSetDashboard(string value)
        {
            var target = new BaselineProviderInput { SuppliedInput = value };

            var result = target.Validate(new[] { Reporter.Dashboard }, true);

            result.ShouldBe(BaselineProvider.Dashboard);
        }

        [Theory]
        [InlineData("azurefilestorage")]
        [InlineData("AzureFileStorage")]
        public void ShouldSetAzureFileStorage(string value)
        {
            var target = new BaselineProviderInput { SuppliedInput = value };

            var result = target.Validate(new[] { Reporter.Dashboard }, true);

            result.ShouldBe(BaselineProvider.AzureFileStorage);
        }

        [Fact]
        public void ShouldThrowException_OnInvalidInput()
        {
            var target = new BaselineProviderInput { SuppliedInput = "invalid" };

            var exception = Should.Throw<InputException>(() => target.Validate(new[] { Reporter.Dashboard }, true));

            exception.Message.ShouldBe("Baseline storage provider 'invalid' does not exist");
        }
    }
}
