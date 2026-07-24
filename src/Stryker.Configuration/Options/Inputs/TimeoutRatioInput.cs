using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class TimeoutRatioInput : Input<double?>
{
    public override double? Default => 1.5;

    protected override string Description => @"The ratio the estimated test time is multiplied by when calculating the timeout for a mutant.
A timeout is calculated per mutant based on the initial unit test run before mutating.
Increase this value if you experience a lot of timeouts, decrease it to catch endless loops faster.";

    public double Validate()
    {
        if (SuppliedInput.HasValue)
        {
            if (SuppliedInput <= 1)
            {
                throw new InputException("Timeout ratio must be higher than 1.");
            }
            return SuppliedInput.Value;
        }

        return Default.Value;
    }
}
