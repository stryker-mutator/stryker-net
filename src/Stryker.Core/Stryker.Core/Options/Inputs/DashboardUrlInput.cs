namespace Stryker.Core.Options.Inputs;
using System;
using Stryker.Core.Exceptions;

public class DashboardUrlInput : Input<string>
{
    public static readonly string DefaultUrl = "https://dashboard.stryker-mutator.io";
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
