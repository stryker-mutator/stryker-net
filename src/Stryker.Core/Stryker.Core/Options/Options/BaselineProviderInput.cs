using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class BaselineProviderInput : ComplexStrykerInput<BaselineProvider, string>
    {
        static BaselineProviderInput()
        {
            HelpText = "Allows to choose a storage location for the baseline reports";
            DefaultValue = BaselineProvider.Disk;
        }

        public override StrykerInput Type => StrykerInput.BaselineProvider;

        public BaselineProviderInput(string baselineProviderLocation, bool dashboardReporterEnabled)
        {
            if (baselineProviderLocation is null && dashboardReporterEnabled)
            {
                Value = BaselineProvider.Dashboard;
            }
            else if (baselineProviderLocation is { })
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
    }
}
