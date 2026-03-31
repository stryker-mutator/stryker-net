using Stryker.Abstractions.Baseline;

namespace Stryker.Configuration.Options.Inputs;

public class S3RegionInput : Input<string>
{
    protected override string Description => "The region for the S3 bucket (e.g. us-east-1). Optional; when not set the SDK resolves the region from environment variables or profile.";

    public override string Default => string.Empty;

    public string Validate(BaselineProvider baselineProvider, bool withBaseline)
    {
        if (withBaseline && baselineProvider == BaselineProvider.S3 && !string.IsNullOrWhiteSpace(SuppliedInput))
        {
            return SuppliedInput;
        }
        return Default;
    }
}
