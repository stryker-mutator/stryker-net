using Stryker.Abstractions.Baseline;

namespace Stryker.Configuration.Options.Inputs;

public class AwsS3RegionInput : Input<string>
{
    protected override string Description => "The AWS region for the S3 bucket (e.g. us-east-1). Optional; when not set the AWS SDK resolves the region from environment variables or AWS profile.";

    public override string Default => string.Empty;

    public string Validate(BaselineProvider baselineProvider, bool withBaseline)
    {
        if (withBaseline && baselineProvider == BaselineProvider.AWSS3 && !string.IsNullOrWhiteSpace(SuppliedInput))
        {
            return SuppliedInput;
        }
        return Default;
    }
}
