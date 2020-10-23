namespace Stryker.Core.Options
{
    public class Thresholds
    {
        public int High { get; }

        public int Low { get; }

        public int Break { get; }

        public Thresholds(int high, int low, int @break)
        {
            High = high;
            Low = low;
            Break = @break;
        }
    }
}
