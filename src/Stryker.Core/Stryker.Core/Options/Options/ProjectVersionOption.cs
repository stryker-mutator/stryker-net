using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class ProjectVersionOption : BaseStrykerOption<string>
    {
        public ProjectVersionOption(string projectVersion, string fallbackVersion, bool dashboardCompareEnabled, IEnumerable<Reporter> reporters)
        {
            if (dashboardCompareEnabled)
            {
                if (string.IsNullOrEmpty(projectVersion))
                {
                    throw new StrykerInputException("When the compare to dashboard feature is enabled the projectversion is required. Please provide a project version");
                }

                if (fallbackVersion == projectVersion)
                {
                    throw new StrykerInputException("Project version cannot be the same as the fallback version. Please provide a different version for either of them");
                }
            }

            if (reporters.Contains(Reporter.Dashboard))
            {
                if (string.IsNullOrEmpty(projectVersion))
                {
                    throw new StrykerInputException("When the dashboard reporter is enabled the projectversion is required. Please provide a project version");
                }
            }

            Value = projectVersion;
        }

        public override StrykerOption Type => StrykerOption.ProjectVersion;

        public override string HelpText => "Project version used by reporters and dashboard compare. Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.";

        public override string DefaultValue => null;
    }
}
