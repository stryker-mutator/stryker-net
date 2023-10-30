using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class SinceInput : Input<bool?>
    {
        public override bool? Default => false;

        protected override string Description => "Enables diff compare. Only test changed files.";

        public bool Validate(bool? withBaseline)
        {
            if (withBaseline.IsNotNullAndTrue())
            {
                if (SuppliedInput.HasValue && SuppliedInput.Value)
                {
                    throw new InputException("The since and baseline features are mutually exclusive.");
                }

                return true;
            }

            return SuppliedInput ?? false;
        }
    }
}
