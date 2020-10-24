using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class GitDiffTargetInput : SimpleStrykerInput<string>
    {
        static GitDiffTargetInput()
        {
            HelpText = "Sets the source commitish to compare with the current codebase, used for calculating the difference when diff is enabled.";
            DefaultValue = "master";
        }

        public override StrykerInput Type => StrykerInput.GitDiffTarget;

        public GitDiffTargetInput(string gitDiffTarget, bool diffEnabled)
        {
            if (diffEnabled && gitDiffTarget.IsNullOrEmptyInput())
            {
                throw new StrykerInputException("The git diff target cannot be empty when the diff feature is enabled");
            }

            Value = gitDiffTarget ?? DefaultValue;
        }
    }
}
