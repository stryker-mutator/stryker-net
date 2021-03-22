namespace Stryker.Core.Options.Inputs
{
    public class SinceInput : InputDefinition<bool?>
    {
        public override bool? Default => false;

        protected override string Description => "Enables diff compare. Only test changed files.";

        public bool Validate(bool? withBaseline)
        {
            if (withBaseline.IsNotNullAndTrue())
            {
                return true;
            }

            return SuppliedInput.HasValue ? SuppliedInput.Value : false;
        }
    }
}
