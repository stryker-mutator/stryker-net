using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class SinceTargetInput : Input<string>
    {
        public override string Default => "master";
        protected override string Description => "The target branch/commit to compare with the current codebase when the since feature is enabled.";

        public string Validate(bool sinceEnabled)
        {
            if (sinceEnabled)
            {
                if (!string.IsNullOrWhiteSpace(SuppliedInput))
                {
                    return SuppliedInput;
                }

                if(SuppliedInput is null)
                {
                    return Default;
                }
                throw new InputException("The target branch/commit cannot be empty when the since feature is enabled");
            }
            return null;
        }
    }
}
