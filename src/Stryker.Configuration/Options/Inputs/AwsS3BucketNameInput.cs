using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class AwsS3BucketNameInput : Input<string>
{
    protected override string Description => "The name of the AWS S3 bucket to store baseline reports in. Required when the AWSS3 baseline provider is selected.";

    public override string Default => string.Empty;

    public string Validate(BaselineProvider baselineProvider, bool withBaseline)
    {
        if (withBaseline && baselineProvider == BaselineProvider.AWSS3)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("The AWS S3 bucket name is required when AWSS3 is used as the baseline provider.");
            }

            return SuppliedInput;
        }
        return Default;
    }
}
