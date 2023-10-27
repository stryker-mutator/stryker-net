using System;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class AzureFileStorageUrlInput : Input<string>
    {
        protected override string Description => @"The url for the Azure File Storage is only needed when the Azure baseline provider is selected.
The url should look something like this:
https://STORAGE_NAME.file.core.windows.net/FILE_SHARE_NAME
Note, the url might be different depending on where your file storage is hosted.";

        public override string Default => string.Empty;

        public string Validate(BaselineProvider baselineProvider, bool withBaseline)
        {
            if (withBaseline && baselineProvider == BaselineProvider.AzureFileStorage)
            {
                if (SuppliedInput is null)
                {
                    throw new InputException("The Azure File Storage url is required when Azure File Storage is used for dashboard compare.");
                }

                if (!Uri.IsWellFormedUriString(SuppliedInput, UriKind.Absolute))
                {
                    throw new InputException($"The Azure File Storage url is not a valid Uri: {SuppliedInput}");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
