using System.Collections.Generic;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Stryker.Abstractions.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Abstractions.Baseline;

namespace Stryker.Abstractions.UnitTest.Options.Inputs
{
    [TestClass]
    public class BaselineProviderInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new BaselineProviderInput();
            target.HelpText.ShouldBe("Choose a storage location for dashboard compare. Set to Dashboard provider when the dashboard reporter is turned on. | default: 'disk' | allowed: Dashboard, Disk, AzureFileStorage");
        }

        [TestMethod]
        public void ShouldSetDefault_WhenBaselineIsDisabled()
        {
            var target = new BaselineProviderInput { SuppliedInput = "azurefilestorage" };

            var result = target.Validate(new Reporter[] { }, false);

            result.ShouldBe(BaselineProvider.Disk);
        }

        [TestMethod]
        [DataRow(new Reporter[] { })]
        [DataRow(new Reporter[] { Reporter.Baseline })]
        public void ShouldSetDefault_WhenInputIsNullAndDashboardReporterIsNotEnabled(IEnumerable<Reporter> reporters)
        {
            var target = new BaselineProviderInput { SuppliedInput = null };

            var result = target.Validate(reporters, true);

            result.ShouldBe(BaselineProvider.Disk);
        }

        [TestMethod]
        [DataRow(new Reporter[] { Reporter.Dashboard })]
        [DataRow(new Reporter[] { Reporter.Dashboard, Reporter.Baseline })]
        public void ShouldSetDashboard_WhenInputIsNullAndDashboardReporterIsEnabled(IEnumerable<Reporter> reporters)
        {
            var target = new BaselineProviderInput { SuppliedInput = null };

            var result = target.Validate(reporters, true);

            result.ShouldBe(BaselineProvider.Dashboard);
        }

        [TestMethod]
        [DataRow("disk")]
        [DataRow("Disk")]
        public void ShouldSetDisk(string value)
        {
            var target = new BaselineProviderInput { SuppliedInput = value };

            var result = target.Validate(new[] { Reporter.Dashboard }, true);

            result.ShouldBe(BaselineProvider.Disk);
        }

        [TestMethod]
        [DataRow("dashboard")]
        [DataRow("Dashboard")]
        public void ShouldSetDashboard(string value)
        {
            var target = new BaselineProviderInput { SuppliedInput = value };

            var result = target.Validate(new[] { Reporter.Dashboard }, true);

            result.ShouldBe(BaselineProvider.Dashboard);
        }

        [TestMethod]
        [DataRow("azurefilestorage")]
        [DataRow("AzureFileStorage")]
        public void ShouldSetAzureFileStorage(string value)
        {
            var target = new BaselineProviderInput { SuppliedInput = value };

            var result = target.Validate(new[] { Reporter.Dashboard }, true);

            result.ShouldBe(BaselineProvider.AzureFileStorage);
        }

        [TestMethod]
        public void ShouldThrowException_OnInvalidInput()
        {
            var target = new BaselineProviderInput { SuppliedInput = "invalid" };

            var exception = Should.Throw<InputException>(() => target.Validate(new[] { Reporter.Dashboard }, true));

            exception.Message.ShouldBe("Baseline storage provider 'invalid' does not exist");
        }
    }
}
