using Stryker.Core.Exceptions;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    // This does not work because of the helptext
    public class ThresholdsOption : BaseStrykerOption<Thresholds>
    {
        public ThresholdsOption(int? high, int? low, int? @break)
        {
            if (high is { })
            {
                Value = new Thresholds(high.Value, Value.Low, Value.Break);
            }
            if (low is { })
            {
                Value = new Thresholds(Value.High, low.Value, Value.Break);
            }
            if (@break is { })
            {
                Value = new Thresholds(Value.High, Value.Low, @break.Value);
            }

            if (new[] { Value.High, Value.Low, Value.Break }.Any(x => x > 100 || x < 0))
            {
                throw new StrykerInputException("The thresholds must be between 0 and 100.");
            }

            // ThresholdLow and ThresholdHigh can be the same value
            if (Value.Break >= Value.Low || Value.Low > Value.High)
            {
                throw new StrykerInputException("The values of your thresholds are incorrect. Change threshold break to the lowest and threshold high to the highest value");
            }
        }

        public override StrykerOption Type => StrykerOption.Thresholds;
        public override string HelpText => @"Set the thresholds depending the minimum, Lower bound and the preferred mutation score threshold.
    Anything below the minimum score will return a non-zero exit code.";
        public override Thresholds DefaultValue => new Thresholds(80, 60, 0);
    }
}
