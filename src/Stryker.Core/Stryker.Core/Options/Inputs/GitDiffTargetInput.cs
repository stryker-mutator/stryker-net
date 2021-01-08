using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class GitDiffTargetInput : OptionDefinition<string>
    {
        public override StrykerOption Type => StrykerOption.DiffTarget;
        public override string DefaultValue => "master";
        protected override string Description => "The target commitish to compare with the current codebase when a diff feature is enabled.";
        protected override string HelpOptions => DefaultInput;

        public GitDiffTargetInput() { }
        public GitDiffTargetInput(string gitDiffTarget, bool diffEnabled)
        {
            if (gitDiffTarget is { } && diffEnabled)
            {
                if (gitDiffTarget.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("The git diff target cannot be empty when the diff feature is enabled");
                }
                Value = gitDiffTarget;
            }
        }
    }
}
