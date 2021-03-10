using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class AzureFileStorageSasInput : InputDefinition<string>
    {
        protected override string Description => "A Shared Access Signature for Azure File Storage is required when azure file storage is used for dashboard compare. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview";

        public override string Default => string.Empty;

        public string Validate(BaselineProvider baselineProvider)
        {
            if (baselineProvider == BaselineProvider.AzureFileStorage)
            {
                if (string.IsNullOrWhiteSpace(SuppliedInput))
                {
                    throw new StrykerInputException("The azure file storage shared access signature is required when azure file storage is used for dashboard compare.");
                }

                // Normalize the SAS
                if (SuppliedInput.StartsWith("?sv="))
                {
                    return SuppliedInput.Replace("?sv=", "");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
