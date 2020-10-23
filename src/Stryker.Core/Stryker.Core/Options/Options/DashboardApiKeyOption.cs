using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class DashboardApiKeyOption : BaseStrykerOption<string>
    {
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

        public override StrykerOption Type => StrykerOption.DashboardApiKey;

        public override string HelpText => $"The api key for the stryker dashboard. You can get your key here: https://dashboard.stryker-mutator.io/.";
    }
}
