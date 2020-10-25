using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Inputs
{
    public class DashboardUrlInput : SimpleStrykerInput<string>
    {
        static DashboardUrlInput()
        {
            HelpText = $"Provide an alternative root url for Stryker Dashboard.";
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
