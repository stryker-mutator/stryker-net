using System;
using System.Collections.Generic;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DashboardApiKeyInputTests
    {
        const string StrykerDashboardApiKey = "STRYKER_DASHBOARD_API_KEY";

        [Fact]
        public void ShouldThrowWhenNull()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, string.Empty);

                var ex = Should.Throw<InputException>(() =>
                {
                    target.Validate(true);
                });
                ex.Message.ShouldContain($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {DashboardUrlInput.DefaultUrl}");
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }

        [Fact]
        public void ShouldSkipValidationWhenDashboardNotEnabled()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, string.Empty);

                var result = target.Validate(false);

                result.ShouldBeNull();
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }

        [Fact]
        public void ShouldTakeEnvironmentVariableValueWhenAvailable()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, "my key");

                var result = target.Validate(true);

                result.ShouldBe("my key");
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }

        [Fact]
        public void ShouldOverrideEnvironmentVariableWhenInputSupplied()
        {
            var key = Environment.GetEnvironmentVariable(StrykerDashboardApiKey);
            var target = new DashboardApiKeyInput();
            target.SuppliedInput = "my key";
            try
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, "not my key");

                var result = target.Validate(true);

                result.ShouldBe("my key");
            }
            finally
            {
                Environment.SetEnvironmentVariable(StrykerDashboardApiKey, key);
            }
        }
    }
}
