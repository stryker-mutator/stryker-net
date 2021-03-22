using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class SinceBranchInput : InputDefinition<string>
    {
        public override string Default => "master";
        protected override string Description => "The target branch to compare with the current codebase when the since feature is enabled.";

        public string Validate(bool sinceEnabled)
        {
            if (SuppliedInput is { } && sinceEnabled)
            {
                if (SuppliedInput.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("The since branch cannot be empty when the since feature is enabled");
                }
                return SuppliedInput;
            }
            return null;
        }
    }
}
