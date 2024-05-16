using Stryker.Shared.Options;

namespace Stryker.Core.Options;

public class Thresholds : IThresholds
{
    public int High { get; init; }

    public int Low { get; init; }

    public int Break { get; init; }
}
