namespace Stryker.Core.Options.Inputs
{
    public class DiffCompareInput : InputDefinition<bool>
    {
        public override bool Default => false;

        protected override string Description => "Enables diff compare. Only test changed files.";

        public DiffCompareInput() { }
        public DiffCompareInput(bool? diffEnabled)
        {
            if (diffEnabled is { })
            {
                Value = diffEnabled.Value;
            }
        }
    }
}
