namespace Stryker.Core.Options.Options
{
    public class DiffEnabledOption : BaseStrykerOption<bool>
    {
        public DiffEnabledOption(bool? diffEnabled)
        {
            Value = diffEnabled ?? DefaultValue;
        }

        public override StrykerOption Type => StrykerOption.DiffEnabled;
        public override string HelpText => "Enables the diff feature. It makes sure to only mutate changed files.";
    }
}
