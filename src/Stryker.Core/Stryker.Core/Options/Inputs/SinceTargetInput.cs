using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class SinceTargetInput : InputDefinition<string>
    {
        public override string Default => "master";
        protected override string Description => "The target branch/commit to compare with the current codebase when the since feature is enabled.";

        public string Validate(bool sinceEnabled)
        {
            if (SuppliedInput is not null && sinceEnabled)
            {
                if (SuppliedInput.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("The target branch/commit cannot be empty when the since feature is enabled");
                }
                return SuppliedInput;
            }
            return null;
        }
    }
}
