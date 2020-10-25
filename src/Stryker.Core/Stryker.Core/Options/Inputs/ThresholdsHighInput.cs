using Stryker.Core.Exceptions;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    // This does not work because of the helptext
    public class ThresholdsHighInput : SimpleStrykerInput<int>
    {
        static ThresholdsHighInput()
        {
            HelpText = $"Set the preferred mutation score threshold. | {DefaultValue} (default)";
            DefaultValue = 80;
        }

        public override StrykerInput Type => StrykerInput.ThresholdsHigh;

        public ThresholdsHighInput(int? high, int? low)
        {
            if (high is { })
            {
                Value = (int)high;
            }

            if (high > 100 || high < 0)
            {
                throw new StrykerInputException("The thresholds must be between 0 and 100.");
            }

            // ThresholdLow and ThresholdHigh can be the same value
            if (low > high)
            {
                throw new StrykerInputException("The values of your thresholds are incorrect. The lowwer bound is currently higher than the lower bound");
            }
        }
    }
}
