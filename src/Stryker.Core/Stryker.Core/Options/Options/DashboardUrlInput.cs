using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Options
{
    public class DashboardUrlInput : SimpleStrykerInput<string>
    {
        static DashboardUrlInput()
        {
            HelpText = "The url to the stryker dashboard. If you're not using the official hosted stryker dashboard, provide the private url here.";
            DefaultValue = "https://dashboard.stryker-mutator.io";
        }

        public override StrykerInput Type => StrykerInput.DashboardUrl;

        public DashboardUrlInput(string url)
        {
            if (url is { })
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    throw new StrykerInputException("Stryker dashboard url {0} is invalid.", url);
                }

                Value = url;
            }
        }
    }
}
