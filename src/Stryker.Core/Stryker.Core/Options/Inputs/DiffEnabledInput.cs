namespace Stryker.Core.Options.Inputs
{
    public class DiffEnabledInput : SimpleStrykerInput<bool>
    {
        public override StrykerInput Type => StrykerInput.DiffCompare;
        public override bool DefaultValue => false;

        protected override string Description => "Enables the diff feature. It makes sure to only mutate changed files. Gets the diff from git by default.";

        public DiffEnabledInput(bool? diffEnabled)
        {
            if (diffEnabled is { })
            {
                Value = diffEnabled.Value;
            }
        }
    }
}
