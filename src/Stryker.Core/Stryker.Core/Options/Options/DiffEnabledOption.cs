namespace Stryker.Core.Options.Options
{
    public class DiffEnabledOption : BaseStrykerOption<bool>
    {
        static DiffEnabledOption()
        {
            HelpText = "Enables the diff feature. It makes sure to only mutate changed files.";
        }

        public override StrykerOption Type => StrykerOption.DiffEnabled;

        public DiffEnabledOption(bool? diffEnabled)
        {
            Value = diffEnabled ?? DefaultValue;
        }
    }
}
