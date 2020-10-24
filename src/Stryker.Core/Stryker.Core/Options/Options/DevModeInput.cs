namespace Stryker.Core.Options.Options
{
    public class DevModeInput : SimpleStrykerInput<bool>
    {
        static DevModeInput()
        {
            HelpText = @"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
    Setting this flag makes stryker not remove the mutations but rather break on failed rollbacks";
            DefaultValue = false;
        }

        public override StrykerInput Type => StrykerInput.DevMode;

        public DevModeInput(bool? devMode)
        {
            Value = devMode ?? DefaultValue;
        }
    }
}
