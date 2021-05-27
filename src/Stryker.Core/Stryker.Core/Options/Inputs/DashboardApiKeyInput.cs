using System;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs
{
    public class DashboardApiKeyInput : InputDefinition<string>
    {
        protected override string Description => "Api key for dashboard reporter.";

        public override string Default => null;

        public string Validate(bool? dashboardEnabled)
        {
            if (!dashboardEnabled.IsNotNullAndTrue())
            {
                // this input is not needed when dashboard is not enabled
                return Default;
            }
            if (SuppliedInput.IsNullOrEmptyInput())
            {
                var environmentApiKey = Environment.GetEnvironmentVariable("STRYKER_DASHBOARD_API_KEY");
                if (!string.IsNullOrWhiteSpace(environmentApiKey))
                {
                    return environmentApiKey;
                }
                else
                {
                    throw new InputException($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {DashboardUrlInput.DefaultUrl}");
                }
            }

            return SuppliedInput;
        }
    }
}
