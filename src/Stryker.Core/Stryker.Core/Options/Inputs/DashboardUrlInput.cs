using System;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class DashboardUrlInput : InputDefinition<string>
    {
        public static string DefaultUrl = "https://dashboard.stryker-mutator.io";
        public override string Default => DefaultUrl;

        protected override string Description => "Alternative url for Stryker Dashboard.";

        public string Validate()
        {
            if (SuppliedInput is { })
            {
                if (!Uri.IsWellFormedUriString(SuppliedInput, UriKind.Absolute))
                {
                    throw new InputException("Stryker dashboard url {0} is invalid.", SuppliedInput);
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
