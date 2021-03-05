using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    // This does not work because of the helptext
    public class ThresholdLowInput : OptionDefinition<int>
    {
        public override int DefaultValue => 60;

        protected override string Description => "Minimum acceptable mutation score. Must be less than or equal to threshold high and more than threshold break.";
        protected override string HelpOptions => FormatHelpOptions("0 - 100");

        public ThresholdLowInput() { }
        public ThresholdLowInput(int? lowInput, int @break, int high)
        {
            if (lowInput is { })
            {
                var low = lowInput.Value;
                if (low > 100 || low < 0)
                {
                    throw new StrykerInputException("Threshold low must be between 0 and 100.");
                }

                if (low > high || low < @break)
                {
                    throw new StrykerInputException($"Threshold low must be less than or equal to threshold high and more than threshold break. Current high: {high}, low: {low}, break: {@break}");
                }

                Value = low;
            }
        }
    }
}
