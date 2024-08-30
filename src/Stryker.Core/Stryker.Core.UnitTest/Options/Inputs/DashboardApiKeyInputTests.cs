using System;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Stryker.Abstractions.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Abstractions.Baseline;

namespace Stryker.Abstractions.UnitTest.Options.Inputs
{
    [TestClass]
    public class DashboardApiKeyInputTests : TestBase
    {
        const string StrykerDashboardApiKey = "STRYKER_DASHBOARD_API_KEY";

        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new DashboardApiKeyInput();
            target.HelpText.ShouldBe(@"Api key for dashboard reporter.");
        }

        [TestMethod]
        public void ShouldThrowWhenNull()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, string.Empty);

                var ex = Should.Throw<InputException>(() =>
                {
                    target.Validate(true, BaselineProvider.Dashboard, new[] { Reporter.Dashboard });
                });
                ex.Message.ShouldContain($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {DashboardUrlInput.DefaultUrl}");
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }

        [TestMethod]
        public void ShouldSkipValidationWhenDashboardNotEnabled()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, string.Empty);

                var result = target.Validate(false, BaselineProvider.Disk, new[] { Reporter.ClearText });

                result.ShouldBeNull();
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }

        [TestMethod]
        public void ShouldTakeEnvironmentVariableValueWhenAvailable()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, "my key");

                var result = target.Validate(true, BaselineProvider.Dashboard, new[] { Reporter.Dashboard });

                result.ShouldBe("my key");
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }

        [TestMethod]
        public void ShouldOverrideEnvironmentVariableWhenInputSupplied()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            target.SuppliedInput = "my key";
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, "not my key");

                var result = target.Validate(true, BaselineProvider.Dashboard, new[] { Reporter.Dashboard });

                result.ShouldBe("my key");
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }
    }
}
