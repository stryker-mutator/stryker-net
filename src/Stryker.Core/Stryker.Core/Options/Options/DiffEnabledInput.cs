namespace Stryker.Core.Options.Options
{
    public class DiffEnabledInput : SimpleStrykerInput<bool>
    {
        static DiffEnabledInput()
        {
            HelpText = @"Enables the diff feature. It makes sure to only mutate changed files. Gets the diff from git by default.";
        }

        public override StrykerInput Type => StrykerInput.DiffEnabled;

        public DiffEnabledInput(bool? diffEnabled)
        {
            Value = diffEnabled ?? DefaultValue;
        }
    }
}
