using System.Collections.Generic;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class ThresholdLowInput : Input<int?>
{
    public override int? Default => 60;

    protected override string Description => "Minimum acceptable mutation score. Must be less than or equal to threshold high and more than or equal to threshold break.";
    protected override IEnumerable<string> AllowedOptions => new[] { "0 - 100" };

    public int Validate(int? @break, int? high)
    {
        if (SuppliedInput is not null)
        {
            var low = SuppliedInput.Value;
            if (low > 100 || low < 0)
            {
                throw new InputException("Threshold low must be between 0 and 100.");
            }

            if (low > high)
            {
                throw new InputException($"Threshold low must be less than or equal to threshold high. Current low: {low}, high: {high}.");
            }

            if(low < @break)
            {
                throw new InputException($"Threshold low must be more than or equal to threshold break. Current low: {low}, break: {@break}.");
            }

            return low;
        }
        return Default.Value;
    }
}
