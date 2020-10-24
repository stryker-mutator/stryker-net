using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class DashboardApiKeyOption : BaseStrykerOption<string>
    {
        static DashboardApiKeyOption()
        {
            HelpText = $"The api key for the stryker dashboard. You can get your key here: https://dashboard.stryker-mutator.io/.";
        }

        public override StrykerOption Type => StrykerOption.DashboardApiKey;

        public DashboardApiKeyOption(string apiKey, bool dashboardEnabled)
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
