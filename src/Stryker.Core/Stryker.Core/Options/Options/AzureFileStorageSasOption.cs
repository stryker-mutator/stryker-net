using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class AzureFileStorageSasOption : BaseStrykerOption<string>
    {
        public AzureFileStorageSasOption(string azureFileStorageSas, BaselineProvider baselineProvider)
        {
            if (baselineProvider == BaselineProvider.AzureFileStorage)
            {
                if (azureFileStorageSas == null)
                {
                    throw new StrykerInputException("The azure file storage shared access signature is required when the azure file storage baseline location is chosen.");
                }

                Value = azureFileStorageSas;

                // Normalize the SAS
                if (azureFileStorageSas.StartsWith("?sv="))
                {
                    Value = azureFileStorageSas.Replace("?sv=", "");
                }
            }
        }

        public override StrykerOption Type => StrykerOption.AzureFileStorageSas;

        public override string HelpText => "The Shared Access Signature for Azure File Storage, only needed when the azure baseline provider is selected. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview";

        public override string DefaultValue => null;
    }
}
