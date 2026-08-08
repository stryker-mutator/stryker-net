using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class AdditionalTimeoutInput : Input<int?>
{
    public override int? Default => 1000;

    protected override string Description => @"A number of milliseconds that is added to the calculated timeout value for each mutant.
A timeout is calculated per mutant based on the initial unit test run before mutating.
To prevent infinite loops Stryker cancels a testrun if it runs longer than the timeout value.
If you experience a lot of timeouts you might need to increase the timeout value.";

    public int Validate()
    {
        if (SuppliedInput.HasValue)
        {
            if (SuppliedInput < 0)
            {
                throw new InputException("Timeout cannot be negative.");
            }
            return SuppliedInput.Value;
        }
        else
        {
            return Default.Value;
        }
    }
}
