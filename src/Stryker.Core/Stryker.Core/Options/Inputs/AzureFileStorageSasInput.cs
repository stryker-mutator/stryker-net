using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class AzureFileStorageSasInput : OptionDefinition<string>
    {
        protected override string Description => "A Shared Access Signature for Azure File Storage is required when azure file storage is used for dashboard compare. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview";

        public AzureFileStorageSasInput() { }
        public AzureFileStorageSasInput(string azureFileStorageSas, BaselineProvider baselineProvider)
        {
            if (baselineProvider == BaselineProvider.AzureFileStorage)
            {
                if (string.IsNullOrWhiteSpace(azureFileStorageSas))
                {
                    throw new StrykerInputException("The azure file storage shared access signature is required when azure file storage is used for dashboard compare.");
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
