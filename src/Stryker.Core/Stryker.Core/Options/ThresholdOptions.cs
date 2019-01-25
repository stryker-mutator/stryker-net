namespace Stryker.Core.Options
{
    public class ThresholdOptions
    {
        public int ThresholdHigh { get; set; }

        public int ThresholdLow { get; set; }

        public int ThresholdBreak { get; set; }

        public ThresholdOptions(int thresholdHigh, int thresholdLow, int thresholdBreak)
        {
            ThresholdHigh = thresholdHigh;
            ThresholdLow = thresholdLow;
            ThresholdBreak = thresholdBreak;
        }
    }
}
