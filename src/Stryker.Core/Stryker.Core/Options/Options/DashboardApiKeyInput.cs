using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class DashboardApiKeyInput : SimpleStrykerInput<string>
    {
        static DashboardApiKeyInput()
        {
            HelpText = $"The api key for the stryker dashboard. You can get your key here: https://dashboard.stryker-mutator.io/.";
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
