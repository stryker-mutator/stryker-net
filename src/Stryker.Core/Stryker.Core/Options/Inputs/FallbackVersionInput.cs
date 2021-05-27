using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class FallbackVersionInput : InputDefinition<string>
    {

        protected override string Description => @"Project version used as a fallback when no report could be found based on Git information for the compare feature.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
When you don't specify a fallback version the git diff target will be used as fallback version.
Example: If the current branch is based on the master branch, set 'master' as the fallback version";

        public override string Default => string.Empty;

        public string Validate(string sinceTarget, bool? dashboardEnabled)
        {
            if (dashboardEnabled.IsNotNullAndTrue())
            {
                if (SuppliedInput == sinceTarget)
                {
                    throw new InputException("Fallback version cannot be set to the same value as the dashboard-version, please provide a different fallback version");
                }
            }

            return SuppliedInput;
        }
    }
}
