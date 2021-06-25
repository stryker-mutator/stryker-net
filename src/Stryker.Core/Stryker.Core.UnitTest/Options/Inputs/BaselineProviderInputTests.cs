using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class BaselineProviderInputTests
    {
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
    }
}
