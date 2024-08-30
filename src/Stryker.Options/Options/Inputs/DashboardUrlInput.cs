using System;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Abstractions.Options.Inputs
{
    public class DashboardUrlInput : Input<string>
    {
        public static readonly string DefaultUrl = "https://dashboard.stryker-mutator.io"; //NOSONAR: hard coded URL is a default value for an overridable parameter
        public override string Default => DefaultUrl;

        protected override string Description => "Alternative url for Stryker Dashboard.";

        public string Validate()
        {
            if (SuppliedInput is not null)
            {
                if (!Uri.IsWellFormedUriString(SuppliedInput, UriKind.Absolute))
                {
                    throw new InputException($"Stryker dashboard url '{SuppliedInput}' is invalid.");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
