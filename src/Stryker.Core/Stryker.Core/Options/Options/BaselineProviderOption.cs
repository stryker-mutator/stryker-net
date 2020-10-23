using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class BaselineProviderOption : BaseStrykerOption<BaselineProvider>
    {
        public BaselineProviderOption(string baselineProviderLocation, IEnumerable<Reporter> reporters)
        {
            if (baselineProviderLocation is null && reporters.Contains(Reporter.Dashboard))
            {
                Value = BaselineProvider.Dashboard;
            }
            else
            {
                Value = baselineProviderLocation.ToLower() switch
                {
                    "disk" => BaselineProvider.Disk,
                    "dashboard" => BaselineProvider.Dashboard,
                    "azurefilestorage" => BaselineProvider.AzureFileStorage,
                    _ => throw new StrykerInputException("Base line storage provider {0} does not exist", baselineProviderLocation),
                };
            }
        }

        public override StrykerOption Type => StrykerOption.BaselineProvider;
        public override string HelpText => "Allows to choose a storage location for the baseline reports";
        public override BaselineProvider DefaultValue => BaselineProvider.Disk;
    }
}
