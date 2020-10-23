using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class DashboardApiKeyOption : BaseStrykerOption<string>
    {
        public DashboardApiKeyOption(string apiKey, bool dashboardCompareEnabled, IEnumerable<Reporter> reporters)
        {
            if (dashboardCompareEnabled)
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new StrykerInputException("When the compare to dashboard feature is enabled an api key required.");
                }
            }

            if (reporters.Contains(Reporter.Dashboard))
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new StrykerInputException("When the dashboard reporter is enabled an api key is required.");
                }
            }

            Value = apiKey;
        }

        public override StrykerOption Type => StrykerOption.DashboardApiKey;

        public override string HelpText => $"The api key for the stryker dashboard. You can get your key here: https://dashboard.stryker-mutator.io/";

        public override string DefaultValue => null;
    }
}
