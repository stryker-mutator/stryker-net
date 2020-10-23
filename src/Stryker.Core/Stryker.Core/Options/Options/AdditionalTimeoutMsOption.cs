using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    class AdditionalTimeoutMsOption : BaseStrykerOption<int>
    {
        public AdditionalTimeoutMsOption(int additionalTimeoutMs)
        {
            if (additionalTimeoutMs < 0)
            {
                throw new StrykerInputException("Timeout cannot be negative");
            }
            Value = additionalTimeoutMs;
        }

        public override StrykerOption Type => StrykerOption.AdditionalTimeoutMs;
        public override string HelpText => "Stryker calculates a timeout based on the total unit test runtime before mutation plus a buffer in case a mutation increases the runtime for a unit test. To prevent infinite loops stryker cancels the testrun if it runs longer than the timeout value. If you experience a lot of timeout you might need to increase the timeout value.";
        public override int DefaultValue => 5000;
    }
}
