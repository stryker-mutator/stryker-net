using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class AzureFileStorageSasInput : SimpleStrykerInput<string>
    {
        static AzureFileStorageSasInput()
        {
            HelpText = $"The Shared Access Signature for Azure File Storage, only needed when the azure baseline provider is selected. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview";
        }

        public override StrykerInput Type => StrykerInput.AzureFileStorageSas;

        public AzureFileStorageSasInput(string azureFileStorageSas, BaselineProvider baselineProvider)
        {
            if (baselineProvider == BaselineProvider.AzureFileStorage)
            {
                if (string.IsNullOrWhiteSpace(azureFileStorageSas))
                {
                    throw new StrykerInputException("The azure file storage shared access signature is required when azure file storage baseline is selected.");
                }

                Value = azureFileStorageSas;

                // Normalize the SAS
                if (azureFileStorageSas.StartsWith("?sv="))
                {
                    Value = azureFileStorageSas.Replace("?sv=", "");
                }
            }
        }
    }
}
