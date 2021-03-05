using System;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class DashboardUrlInput : InputDefinition<string>
    {
        public override string Default => "https://dashboard.stryker-mutator.io";

        protected override string Description => "Alternative url for Stryker Dashboard.";

        public DashboardUrlInput() { }
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
