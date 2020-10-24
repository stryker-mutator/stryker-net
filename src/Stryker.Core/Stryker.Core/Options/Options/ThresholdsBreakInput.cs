using Stryker.Core.Exceptions;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    // This does not work because of the helptext
    public class ThresholdsBreakInput : SimpleStrykerInput<int>
    {
        static ThresholdsBreakInput()
        {
            HelpText = @"Set the thresholds depending the minimum, Lower bound and the preferred mutation score threshold.
    Anything below the minimum score will return a non-zero exit code.";
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
