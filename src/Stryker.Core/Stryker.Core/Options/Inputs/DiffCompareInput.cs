namespace Stryker.Core.Options.Inputs
{
    public class SinceInput : InputDefinition<bool>
    {
        public override bool Default => false;

        protected override string Description => "Enables diff compare. Only test changed files.";

        public SinceInput() { }
        public SinceInput(bool? diffEnabled)
        {
            if (diffEnabled is { })
            {
                Value = diffEnabled.Value;
            }
        }
    }
}
