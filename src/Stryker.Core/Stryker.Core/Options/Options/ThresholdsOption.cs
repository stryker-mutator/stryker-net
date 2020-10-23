using Stryker.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    class ThresholdsOption : BaseStrykerOption<Threshold>
    {
        public ThresholdsOption(int thresholdHigh, int thresholdLow, int thresholdBreak)
        {
            List<int> thresholdList = new List<int> { thresholdHigh, thresholdLow, thresholdBreak };
            if (thresholdList.Any(x => x > 100 || x < 0))
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "The thresholds must be between 0 and 100");
            }

            // ThresholdLow and ThresholdHigh can be the same value
            if (thresholdBreak >= thresholdLow || thresholdLow > thresholdHigh)
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "The values of your thresholds are incorrect. Change `--threshold-break` to the lowest value and `--threshold-high` to the highest.");
            }

            Value = new Threshold(thresholdHigh, thresholdLow, thresholdBreak);
        }

        public override StrykerOption Type => StrykerOption.Thresholds;
        public override string HelpText => "";
        public override Threshold DefaultValue => new Threshold(80, 60, 0);
    }
}
