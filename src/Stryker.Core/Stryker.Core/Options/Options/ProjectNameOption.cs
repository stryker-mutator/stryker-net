using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class ProjectNameOption : BaseStrykerOption<string>
    {
        static ProjectNameOption()
        {
            HelpText = "The project name for the stryker dashboard. Required when the stryker dashboard is used. Often the name of your solution or repository.";

        }

        public override StrykerOption Type => StrykerOption.ProjectName;

        public ProjectNameOption(string projectName, bool dashboardEnabled)
        {
            if (dashboardEnabled)
            {
                if (projectName.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("When the stryker dashboard is enabled the project name is required.");
                }

                Value = projectName;
            }
        }
    }
}
