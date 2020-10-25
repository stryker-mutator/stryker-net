using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace Stryker.Core.Options.Inputs
{
    public class BaselineProviderInput : ComplexStrykerInput<string, BaselineProvider>
    {
        static BaselineProviderInput()
        {
            HelpText = $@"Allows to choose a storage location | Options[{FormatOptions(DefaultInput, (IEnumerable<BaselineProvider>)Enum.GetValues(DefaultValue.GetType())) }]
                                     When using the azure file storage, make sure to configure the -sas and -storage-url options.";
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
