namespace Stryker.Core.Options.Options
{
    public class DevModeOption : BaseStrykerOption<bool>
    {
        static DevModeOption()
        {
            HelpText = @"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
    Setting this flag makes stryker not remove the mutations but rather break on failed rollbacks";
            DefaultValue = false;
        }

        public override StrykerOption Type => StrykerOption.DevMode;

        public DevModeOption(bool? devMode)
        {
            Value = devMode ?? DefaultValue;
        }
    }
}
