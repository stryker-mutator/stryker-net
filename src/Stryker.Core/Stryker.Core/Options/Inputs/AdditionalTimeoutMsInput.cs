using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class AdditionalTimeoutMsInput : SimpleStrykerInput<int>
    {
        static AdditionalTimeoutMsInput()
        {
            HelpText = $"Stryker calculates a timeout based on the time the testrun takes before the mutations | Options {DefaultValue}";
            DefaultValue = 5000;
        }

        public override StrykerInput Type => StrykerInput.AdditionalTimeoutMs;

        public AdditionalTimeoutMsInput(int? additionalTimeoutMs)
        {
            if (additionalTimeoutMs is { })
            {
                if (additionalTimeoutMs < 0)
                {
                    throw new StrykerInputException("Timeout cannot be negative");
                }
                Value = additionalTimeoutMs.Value;
            }
        }
    }
}
