using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class BaselineTargetInput : Input<string>
    {
        public override string Default => "main";
        protected override string Description => "The target branch/commit to compare with the current codebase when the baseline feature is enabled.";

        public string Validate(bool sinceEnabled)
        {
            if (sinceEnabled && SuppliedInput is not null)
            {
                if (string.IsNullOrWhiteSpace(SuppliedInput))
                {
                    throw new InputException("The baseline target cannot be empty when the since feature is enabled");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
