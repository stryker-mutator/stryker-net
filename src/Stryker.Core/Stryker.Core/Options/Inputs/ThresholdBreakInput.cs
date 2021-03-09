using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    // This does not work because of the helptext
    public class ThresholdBreakInput : InputDefinition<int?>
    {
        public override int? Default => 0;

        protected override string Description => "Anything below this mutation score will return a non-zero exit code. Must be less than threshold low.";
        protected override string HelpOptions => FormatHelpOptions("0 - 99");

        public int Validate(int? low)
        {
            if (SuppliedInput is not null)
            {
                var @break = SuppliedInput.Value;
                if (@break > 99 || @break < 0)
                {
                    throw new StrykerInputException("Threshold break must be between 0 and 99.");
                }

                if (@break <= low)
                {
                    throw new StrykerInputException($"Threshold break must be less than threshold high. Current low: {low}, break: {@break}");
                }

                return @break;
            }
            return Default.Value;
        }
    }
}
