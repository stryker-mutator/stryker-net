using Stryker.Core.Exceptions;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    // This does not work because of the helptext
    public class ThresholdsBreakInput : SimpleStrykerInput<int>
    {
        static ThresholdsBreakInput()
        {
            Description = $"Set the minimum mutation score threshold. Anything below this score will return a non-zero exit code. | {DefaultValue} (default)";
            DefaultValue = 0;
        }

        public override StrykerInput Type => StrykerInput.ThresholdsBreak;

        public ThresholdsBreakInput(int? @break)
        {
            if (@break is { })
            {
                Value = (int)@break;
            }

            if (@break > 100 || @break < 0)
            {
                throw new StrykerInputException("The thresholds must be between 0 and 100.");
            }
        }
    }
}
