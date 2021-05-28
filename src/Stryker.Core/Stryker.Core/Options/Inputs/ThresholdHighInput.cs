using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ThresholdHighInput : InputDefinition<int?>
    {
        public override int? Default => 80;

        protected override string Description => "Minimum good mutation score. Must be higher than or equal to threshold low.";
        protected override string HelpOptions => FormatHelpOptions("1 - 100");

        public int Validate(int? low)
        {
            if (SuppliedInput is not null)
            {
                var high = SuppliedInput.Value;
                if (high > 100 || high < 1)
                {
                    throw new InputException("Threshold high must be between 1 and 100.");
                }

                if (low > high)
                {
                    throw new InputException($"Threshold high must be higher than or equal to threshold low. Current high: {high}, low: {low}.");
                }

                return high;
            }
            return Default.Value;
        }
    }
}
