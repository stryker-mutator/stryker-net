using System;
using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class AzureFileStorageUrlInput : InputDefinition<string>
    {
        protected override string Description => @"The url for the Azure File Storage, only needed when the azure baseline provider is selected. 
                                    The url should look something like this: 
                                    https://STORAGE_NAME.file.core.windows.net/FILE_SHARE_NAME 
                                    Note, the url might be different depending of where your file storage is hosted.";

        public AzureFileStorageUrlInput() { }
        public AzureFileStorageUrlInput(string azureFileStorageUrl)
        {
            if (azureFileStorageUrl is null)
            {
                throw new StrykerInputException("The azure file storage url is required when Azure File Storage is used for dashboard compare.");
            }

            if (!Uri.IsWellFormedUriString(azureFileStorageUrl, UriKind.Absolute))
            {
                throw new StrykerInputException("The azure file storage url is not a valid Uri: {0}", azureFileStorageUrl);
            }

            Value = azureFileStorageUrl;
        }
    }
}
