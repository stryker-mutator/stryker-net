using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Options
{
    public class DashboardUrlOption : BaseStrykerOption<string>
    {
        public DashboardUrlOption(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    throw new StrykerInputException("Stryker dashboard url {0} is invalid.", url);
                }

                Value = url;
            }

            Value = DefaultValue;
        }

        public override StrykerOption Type => StrykerOption.DashboardUrl;

        public override string HelpText => "The url to the stryker dashboard. If you're not using the official hosted stryker dashboard, provide the private url here.";

        public override string DefaultValue => "https://dashboard.stryker-mutator.io";
    }
}
