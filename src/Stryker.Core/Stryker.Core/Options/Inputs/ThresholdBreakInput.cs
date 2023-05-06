using System.Collections.Generic;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class ThresholdBreakInput : Input<int?>
{
    public override int? Default => 0;

    protected override string Description => "Anything below this mutation score will return a non-zero exit code. Must be less than or equal to threshold low.";
    protected override IEnumerable<string> AllowedOptions => new[] { "0 - 100" };

    public int Validate(int? low)
    {
        if (SuppliedInput is not null)
        {
            var @break = SuppliedInput.Value;
            if (@break > 100 || @break < 0)
            {
                throw new InputException("Threshold break must be between 0 and 100.");
            }

            if (@break > low)
            {
                throw new InputException($"Threshold break must be less than or equal to threshold low. Current break: {@break}, low: {low}.");
            }

            return @break;
        }
        return Default.Value;
    }
}
