using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class ProjectVersionOption : BaseStrykerOption<string>
    {
        public ProjectVersionOption(string projectVersion, string fallbackVersion, bool dashboardEnabled, bool dashboardCompareEnabled)
        {
            if (dashboardEnabled)
            {
                if (projectVersion.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("When the stryker dashboard is enabled the projectversion is required. Please provide a project version.");
                }

                if (dashboardCompareEnabled && fallbackVersion == projectVersion)
                {
                    throw new StrykerInputException("Project version cannot be the same as the fallback version. Please provide a different version for either of them.");
                }

                Value = projectVersion;
            }
        }

        public override StrykerOption Type => StrykerOption.ProjectVersion;

        public override string HelpText => "Project version for uploading to stryker dashboard. Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.";
    }
}
