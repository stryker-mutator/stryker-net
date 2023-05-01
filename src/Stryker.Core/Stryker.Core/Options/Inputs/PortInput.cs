using System;

namespace Stryker.Core.Options.Inputs;

public class PortInput : Input<int?>
{
    private const ushort PortMinimum = 0;
    private const ushort PortMaximum = ushort.MaxValue;

    public override int? Default => 8080;
    protected override string Description => "The port used when realtime reporting is enabled";

    public int Validate()
    {
        if (!SuppliedInput.HasValue)
        {
            return Default.Value;
        }

        if (SuppliedInput is < PortMinimum or > PortMaximum or null)
        {
            throw new ArgumentOutOfRangeException($"Port should be between range {PortMinimum} - {PortMaximum}");
        }

        return SuppliedInput.Value;
    }
}
