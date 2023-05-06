using System.Linq;
using System.Web;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class AzureFileStorageSasInput : Input<string>
{
    protected override string Description => "A Shared Access Signature for Azure File Storage is required when Azure File Storage is used for dashboard compare. For more information: https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview";

    public override string Default => string.Empty;

    public string Validate(BaselineProvider baselineProvider)
    {
        if (baselineProvider == BaselineProvider.AzureFileStorage)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("The azure file storage shared access signature is required when azure file storage is used for dashboard compare.");
            }

            var query = HttpUtility.ParseQueryString(SuppliedInput);

            var hasImportantKey = query.AllKeys.Where(x => x.Equals("sv", System.StringComparison.InvariantCultureIgnoreCase) || x.Equals("sig", System.StringComparison.InvariantCultureIgnoreCase));

            if (hasImportantKey.Count() < 2)
            {
                throw new InputException("The azure file storage shared access signature is not in the correct format");
            }

            return SuppliedInput;
        }
        return Default;
    }
}
