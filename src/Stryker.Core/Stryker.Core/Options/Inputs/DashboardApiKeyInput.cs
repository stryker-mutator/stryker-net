using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs;

public class DashboardApiKeyInput : Input<string>
{
    protected override string Description => "Api key for dashboard reporter.";

    public override string Default => null;

    public string Validate(bool? withBaseline, BaselineProvider baselineProvider, IEnumerable<Reporter> reporters)
    {
        /* the dashboard api key is required if 
         * 1: The dashboard reporter is enabled
         * 2: The dasboard storage location is chosen for the with-baseline feature AND the with-baseline feature is enabled
         */
        var dashboardEnabled = (withBaseline.IsNotNullAndTrue() && baselineProvider == BaselineProvider.Dashboard) || reporters.Any(x => x == Reporter.Dashboard);
        if (!dashboardEnabled)
        {
            return Default;
        }
        if (string.IsNullOrWhiteSpace(SuppliedInput))
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
