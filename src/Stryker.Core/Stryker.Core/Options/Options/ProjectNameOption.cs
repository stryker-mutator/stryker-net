using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class ProjectNameOption : BaseStrykerOption<string>
    {
        public ProjectNameOption(string projectName, bool dashboardCompareEnabled, IEnumerable<Reporter> reporters)
        {
            if (dashboardCompareEnabled)
            {
                if (string.IsNullOrWhiteSpace(projectName))
                {
                    throw new StrykerInputException("When the compare to dashboard feature is enabled the project name is required.");
                }
            }

            if (reporters.Contains(Reporter.Dashboard))
            {
                if (string.IsNullOrWhiteSpace(projectName))
                {
                    throw new StrykerInputException("When the dashboard reporter is enabled the project name is required.");
                }
            }

            Value = projectName;
        }

        public override StrykerOption Type => StrykerOption.ProjectName;

        public override string HelpText => "The project name for the stryker dashboard. Required when dashboard reporter or dashboard compare is turned on. Often the name of your solution or repository.";

        public override string DefaultValue => null;
    }
}
