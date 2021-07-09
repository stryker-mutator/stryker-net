using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs
{
    public class BaselineProviderInput : InputDefinition<string, BaselineProvider>
    {
        public override string Default => "disk";

        protected override string Description => "Choose a storage location for dashboard compare. Set to Dashboard provider when the dashboard reporter is turned on.";
        protected override IEnumerable<string> AllowedOptions => EnumToStrings(typeof(BaselineProvider));

        public BaselineProvider Validate(IEnumerable<Reporter> reporters)
        {
            if (SuppliedInput is null && reporters.Contains(Reporter.Dashboard))
            {
                return BaselineProvider.Dashboard;
            }
            else if (SuppliedInput is not null)
            {
                return SuppliedInput.ToLower() switch
                {
                    "disk" => BaselineProvider.Disk,
                    "dashboard" => BaselineProvider.Dashboard,
                    "azurefilestorage" => BaselineProvider.AzureFileStorage,
                    _ => throw new InputException("Baseline storage provider {0} does not exist", SuppliedInput),
                };
            }
            return BaselineProvider.Disk;
        }
    }
}
