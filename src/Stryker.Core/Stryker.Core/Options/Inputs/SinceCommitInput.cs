using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class SinceCommitInput : InputDefinition<string>
    {
        public override string Default => null;
        protected override string Description => "The target commit to compare with the current codebase when the since feature is enabled.";

        public string Validate(bool diffEnabled)
        {
            if (SuppliedInput is { } && diffEnabled)
            {
                if (SuppliedInput.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("The since commit cannot be empty when the since feature is enabled");
                }
                return SuppliedInput;
            }
            return null;
        }
    }
}
