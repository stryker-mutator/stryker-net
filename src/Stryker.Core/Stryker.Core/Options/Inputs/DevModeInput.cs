namespace Stryker.Core.Options.Inputs
{
    public class DevModeInput : SimpleStrykerInput<bool>
    {
        static DevModeInput()
        {
            Description = @"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
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
