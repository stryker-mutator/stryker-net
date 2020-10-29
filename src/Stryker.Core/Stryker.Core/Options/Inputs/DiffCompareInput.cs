namespace Stryker.Core.Options.Inputs
{
    public class DiffCompareInput : SimpleStrykerInput<bool>
    {
        public override StrykerInput Type => StrykerInput.DiffCompare;
        public override bool DefaultValue => false;

        protected override string Description => "Enables diff compare. Only test changed files.";

        public DiffCompareInput(bool? diffEnabled)
        {
            if (diffEnabled is { })
            {
                Value = diffEnabled.Value;
            }
        }
    }
}
