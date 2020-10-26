namespace Stryker.Core.Options.Inputs
{
    public class DiffEnabledInput : SimpleStrykerInput<bool>
    {
        static DiffEnabledInput()
        {
            Description = @"Enables the diff feature. It makes sure to only mutate changed files. Gets the diff from git by default.";
        }

        public override StrykerInput Type => StrykerInput.DiffCompare;

        public DiffEnabledInput(bool? diffEnabled)
        {
            Value = diffEnabled ?? DefaultValue;
        }
    }
}
