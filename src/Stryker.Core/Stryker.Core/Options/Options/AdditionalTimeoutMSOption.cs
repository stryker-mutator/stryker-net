namespace Stryker.Core.Options.Options
{
    class AdditionalTimeoutMSOption : BaseStrykerOption<int>
    {
        public AdditionalTimeoutMSOption(int additionalTimeoutMS)
        {
            Value = additionalTimeoutMS;
        }

        public override StrykerOption Type => StrykerOption.AdditionalTimeoutMS;
        public override string HelpText => "";
        public override int DefaultValue => 5000;
    }
}
