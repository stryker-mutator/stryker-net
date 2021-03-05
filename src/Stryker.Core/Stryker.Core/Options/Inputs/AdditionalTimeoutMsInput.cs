using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class AdditionalTimeoutMsInput : OptionDefinition<int>
    {
        protected override string Description => @"A timeout is calculated based on the initial unit test run before mutating.
To prevent infinite loops stryker cancels a testrun if it runs longer than the timeout value.
If you experience a lot of timeout you might need to increase the timeout value.";

        public AdditionalTimeoutMsInput() { }
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
