using System;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DashboardApiKeyInputTests
    {
        [Fact]
        public void ShouldValidateApiKey()
        {
            const string strykerDashboardApiKey = "STRYKER_DASHBOARD_API_KEY";
            var key = Environment.GetEnvironmentVariable(strykerDashboardApiKey);
            try
            {
                var options = new StrykerOptions();
                Environment.SetEnvironmentVariable(strykerDashboardApiKey, string.Empty);

                var ex = Assert.Throws<StrykerInputException>(() =>
                {
                    new StrykerOptions(reporters: new string[] { "Dashboard" });
                });
                ex.Message.ShouldContain($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {options.DashboardUrl}");
                ex.Message.ShouldContain($"A project name is required when the {Reporter.Dashboard} reporter is turned on!");
            }
            finally
            {
                Environment.SetEnvironmentVariable(strykerDashboardApiKey, key);
            }
        }
    }
}
