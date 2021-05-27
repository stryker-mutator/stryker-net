using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ThresholdBreakInput : InputDefinition<int?>
    {
        public override int? Default => 0;

        protected override string Description => "Anything below this mutation score will return a non-zero exit code. Must be less than or equal to threshold low.";
        protected override string HelpOptions => FormatHelpOptions("0 - 100");

        public int Validate(int? low)
        {
            if (SuppliedInput is not null)
            {
                var @break = SuppliedInput.Value;
                if (@break > 100 || @break < 0)
                {
                    throw new InputException("Threshold break must be between 0 and 100.");
                }

                if (@break > low)
                {
                    throw new InputException($"Threshold break must be less than or equal to threshold low. Current low: {low}, break: {@break}.");
                }

                return @break;
            }
            return Default.Value;
        }
    }
}
