using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class FallbackVersionInput : SimpleStrykerInput<string>
    {
        static FallbackVersionInput()
        {
            Description = $"Project version used as a fallback when no report could be found based on Git information for the Compare feature in reporters. Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing. When you don't specify a fallback version, --git-source will be used as fallback version. Example: If the current branch is based on the master branch, set 'master' as the fallback version";
            DefaultValue = new FallbackVersionInput(DefaultInput, GitDiffTargetInput.DefaultValue).Value;
        }

        public override StrykerInput Type => StrykerInput.FallbackVersion;

        public FallbackVersionInput(string fallbackVersion, string gitDiffTarget)
        {
            if (fallbackVersion.IsNullOrEmptyInput())
            {
                throw new StrykerInputException($"Fallback version cannot be empty. Either fill the option or leave it out.");
            }

            Value = fallbackVersion ?? gitDiffTarget;
        }
    }
}
