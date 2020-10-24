using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Options
{
    public class DashboardUrlOption : BaseStrykerOption<string>
    {
        static DashboardUrlOption()
        {
            HelpText = "The url to the stryker dashboard. If you're not using the official hosted stryker dashboard, provide the private url here.";
            DefaultValue = "https://dashboard.stryker-mutator.io";
        }

        public override StrykerOption Type => StrykerOption.DashboardUrl;

        public DashboardUrlOption(string url)
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
