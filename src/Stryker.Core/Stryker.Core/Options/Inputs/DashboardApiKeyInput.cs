using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class DashboardApiKeyInput : SimpleStrykerInput<string>
    {
        static DashboardApiKeyInput()
        {
            HelpText = $"Api key for dashboard reporter. You can get your key here: {DashboardUrlInput.DefaultValue}";
        }

        public override StrykerInput Type => StrykerInput.DashboardApiKey;

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
