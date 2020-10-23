using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    class GitDiffTargetOption : BaseStrykerOption<string>
    {
        public GitDiffTargetOption(string gitDiffTarget)
        {
            if (string.IsNullOrEmpty(gitDiffTarget))
            {
                throw new StrykerInputException("GitDiffTarget may not be empty, please provide a valid git branch name");
            }
            Value = gitDiffTarget;
        }

        public override StrykerOption Type => StrykerOption.GitDiffTarget;
        public override string HelpText => "Sets the source commitish (branch or commit) to compare with the current codebase, used for calculating the difference when diff is enabled. Default: master";
        public override string DefaultValue => "master";
    }
}
