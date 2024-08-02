using Stryker.Configuration.Options;

namespace Stryker.Configuration
{
    public class Thresholds : IThresholds
    {
        public int High { get; init; }

        public int Low { get; init; }

        public int Break { get; init; }
    }
}
