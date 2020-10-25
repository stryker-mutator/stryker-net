using Stryker.Core.Exceptions;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    // This does not work because of the helptext
    public class ThresholdsLowInput : SimpleStrykerInput<int>
    {
        static ThresholdsLowInput()
        {
            HelpText = $"Set the lower bound of the mutation score threshold. It will not fail the test. | {DefaultValue} (default)";
            DefaultValue = 60;
        }

        public override StrykerInput Type => StrykerInput.ThresholdsLow;

        public ThresholdsLowInput(int? low, int? @break)
        {
            if (low is { })
            {
                Value = (int)low;
            }

            if (low > 100 || low < 0)
            {
                throw new StrykerInputException("The thresholds must be between 0 and 100.");
            }

            // ThresholdLow and ThresholdHigh can be the same value
            if (@break >=low)
            {
                throw new StrykerInputException("The values of your thresholds are incorrect. Change threshold break to the lowest and threshold high to the highest value");
            }
        }
    }
}
