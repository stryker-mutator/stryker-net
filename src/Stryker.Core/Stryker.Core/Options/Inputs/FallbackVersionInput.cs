using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class FallbackVersionInput : Input<string>
    {
        protected override string Description => @"Commitish used as a fallback when no report could be found based on Git information for the baseline feature.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
When you don't specify a fallback version the since target will be used as fallback version.
Example: If the current branch is based on the main branch, set 'main' as the fallback version";

        public override string Default => string.Empty;

        public string Validate(string projectVersion, bool? dashboardEnabled)
        {
            if (dashboardEnabled.IsNotNullAndTrue() && SuppliedInput == projectVersion)
            {
                // Fallback version is used when the current branch cannot be found on the dashboard for the baseline feature. Thus fallback needs to be another version
                throw new InputException("Fallback version cannot be set to the same value as the current project version, please provide a different fallback version");
            }

            return SuppliedInput;
        }
    }
}
