using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class BaselineProviderInput : InputDefinition<string, BaselineProvider>
    {
        public override string DefaultInput => "disk";
        public override BaselineProvider Default => new BaselineProviderInput(DefaultInput, false).Value;

        protected override string Description => "Choose a storage location for dashboard compare. Set to Dashboard provider when the dashboard reporter is turned on.";
        protected override string HelpOptions => FormatEnumHelpOptions();

        public BaselineProviderInput() { }
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
