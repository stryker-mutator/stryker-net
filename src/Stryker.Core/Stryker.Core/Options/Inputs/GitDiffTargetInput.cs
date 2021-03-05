using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class SinceBranchInput : InputDefinition<string>
    {
        public override string Default => "master";
        protected override string Description => "The target branch to compare with the current codebase when the since feature is enabled.";
        protected override string HelpOptions => DefaultInput;

        public SinceBranchInput() { }
        public SinceBranchInput(string gitDiffTarget, bool diffEnabled)
        {
            if (gitDiffTarget is { } && diffEnabled)
            {
                if (gitDiffTarget.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("The since branch target cannot be empty when the since feature is enabled");
                }
                Value = gitDiffTarget;
            }
        }
    }
}
