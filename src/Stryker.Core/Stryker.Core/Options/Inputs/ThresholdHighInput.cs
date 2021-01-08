using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    // This does not work because of the helptext
    public class ThresholdHighInput : OptionDefinition<string, int>
    {
        public override StrykerOption Type => StrykerOption.ThresholdHigh;
        public override string DefaultInput => DefaultValue.ToString();
        public override int DefaultValue => 80;

        protected override string Description => "Minimum good mutation score. Must be higher than or equal to threshold low.";
        protected override string HelpOptions => FormatHelpOptions("0 - 100");

        public ThresholdHighInput() { }
        public ThresholdHighInput(string highInput, int low)
        {
            if (highInput is { })
            {
                var high = int.Parse(highInput);
                if (high > 100 || high < 0)
                {
                    throw new StrykerInputException("Threshold high must be between 0 and 100.");
                }

                if (low > high)
                {
                    throw new StrykerInputException($"Threshold high must be higher than or equal to threshold low. Current high: {high}, low: {low}");
                }

                Value = high;
            }
        }
    }
}
