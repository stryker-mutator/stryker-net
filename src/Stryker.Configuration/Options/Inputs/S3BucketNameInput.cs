using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class S3BucketNameInput : Input<string>
{
    protected override string Description => "The name of the S3 bucket to store baseline reports in. Required when the S3 baseline provider is selected.";

    public override string Default => string.Empty;

    public string Validate(BaselineProvider baselineProvider, bool withBaseline)
    {
        if (withBaseline && baselineProvider == BaselineProvider.S3)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("The S3 bucket name is required when S3 is used as the baseline provider.");
            }

            return SuppliedInput;
        }
        return Default;
    }
}
