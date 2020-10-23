using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Options
{
    public class AzureFileStorageUrlOption : BaseStrykerOption<string>
    {
        public AzureFileStorageUrlOption(string azureFileStorageUrl, BaselineProvider baselineProvider) : base(azureFileStorageUrl)
        {
            if (baselineProvider == BaselineProvider.AzureFileStorage && azureFileStorageUrl == null)
            {
                throw new StrykerInputException("The url pointing to your file storage is required when Azure File Storage is enabled.");
            }
        }

        public override StrykerOption Type => StrykerOption.AzureFileStorageUrl;

        public override string Name => GetType().Name;

        public override string HelpText => @"The url for the Azure File Storage, only needed when the azure baseline provider is selected. 
                                    The url should look something like this: 

                                    https://STORAGE_NAME.file.core.windows.net/FILE_SHARE_NAME 

                                    Note, the url might be different depending of where your file storage is hosted.";

        public override string DefaultValue => null;

        protected override void Validate(params string[] parameters)
        {
            foreach (var param in parameters)
            {
                if (!Uri.IsWellFormedUriString(param, UriKind.Absolute))
                {
                    throw new StrykerInputException("The azure file storage url is not a valid Uri: {0}", param);
                }
            }
        }
    }
}
