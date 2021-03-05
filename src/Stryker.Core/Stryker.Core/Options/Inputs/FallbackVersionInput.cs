using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class FallbackVersionInput : OptionDefinition<string>
    {

        protected override string Description => @"Project version used as a fallback when no report could be found based on Git information for the compare feature.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
When you don't specify a fallback version the git diff target will be used as fallback version.
Example: If the current branch is based on the master branch, set 'master' as the fallback version";

        public FallbackVersionInput() { }
        public FallbackVersionInput(string fallbackVersion, string gitDiffTarget)
        {
            if (fallbackVersion is { } && fallbackVersion.IsNullOrEmptyInput())
            {
                throw new StrykerInputException($"Fallback version cannot be empty. Either fill the option or leave it out to use the default.");
            }

            Value = fallbackVersion ?? gitDiffTarget;
        }
    }
}
