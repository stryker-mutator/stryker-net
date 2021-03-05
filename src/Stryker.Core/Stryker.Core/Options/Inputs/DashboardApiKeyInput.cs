using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class DashboardApiKeyInput : InputDefinition<string>
    {
        protected override string Description => "Api key for dashboard reporter.";

        public DashboardApiKeyInput() { }
        public DashboardApiKeyInput(string apiKey, bool dashboardEnabled)
        {
            if (dashboardEnabled)
            {
                if (apiKey.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("When the stryker dashboard is enabled an api key required.");
                }

                Value = apiKey;
            }
        }
    }
}
