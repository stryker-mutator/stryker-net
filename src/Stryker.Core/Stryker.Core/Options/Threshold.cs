namespace Stryker.Core.Options
{
    public class Threshold
    {
        public int High { get; set; }

        public int Low { get; set; }

        public int Break { get; set; }

        public Threshold(int high, int low, int @break)
        {
            High = high;
            Low = low;
            Break = @break;
        }
    }
}
