using Stryker.Abstractions.Options;

namespace Stryker.Abstractions;

public class Thresholds : IThresholds
{
    public int High { get; init; }

    public int Low { get; init; }

    public int Break { get; init; }
}
