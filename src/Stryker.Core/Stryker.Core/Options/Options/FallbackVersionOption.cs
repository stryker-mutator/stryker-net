using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class FallbackVersionOption : BaseStrykerOption<string>
    {
        public FallbackVersionOption(string fallbackVersion, string gitDiffTarget)
        {
            if (fallbackVersion == "")
            {
                throw new StrykerInputException("Fallback version cannot be empty. Either fill the option or leave it out.");
            }

            Value = fallbackVersion ?? gitDiffTarget;
        }

        public override StrykerOption Type => StrykerOption.FallbackVersion;

        public override string HelpText => "Report version used when no report could be found for the project version. Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing. When you don't specify a fallback version, the GitTarget option will be used as fallback version. Example: If the current branch is based on the master branch, set 'master' as the fallback version";

        public override string DefaultValue => null;
    }
}
