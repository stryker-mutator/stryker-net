using System;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class S3EndpointInput : Input<string>
{
    protected override string Description => "Custom endpoint URL for S3-compatible storage (e.g. MinIO, Backblaze B2). Optional; when not set the default AWS S3 endpoint is used.";

    public override string Default => string.Empty;

    public string Validate(BaselineProvider baselineProvider, bool withBaseline)
    {
        if (withBaseline && baselineProvider == BaselineProvider.S3 && !string.IsNullOrWhiteSpace(SuppliedInput))
        {
            if (!Uri.IsWellFormedUriString(SuppliedInput, UriKind.Absolute))
            {
                throw new InputException($"The S3 endpoint is not a valid Uri: {SuppliedInput}");
            }

            return SuppliedInput;
        }
        return Default;
    }
}
