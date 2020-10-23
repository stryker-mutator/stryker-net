namespace Stryker.Core.Options.Options
{
    class AdditionalTimeoutMSOption : BaseStrykerOption<int>
    {
        public AdditionalTimeoutMSOption(int additionalTimeoutMS)
        {
            Value = additionalTimeoutMS;
        }

        public override StrykerOption Type => StrykerOption.AdditionalTimeoutMS;
        public override string HelpText => "Stryker calculates a timeout based on the time the testrun takes before the mutations";
        public override int DefaultValue => 5000;
    }
}
