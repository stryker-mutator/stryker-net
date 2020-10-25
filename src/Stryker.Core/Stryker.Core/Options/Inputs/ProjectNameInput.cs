using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectNameInput : SimpleStrykerInput<string>
    {
        static ProjectNameInput()
        {
            HelpText = @"The organizational name for your project. Required when dashboard reporter is turned on.
For example: Your project might be called 'consumer-loans' and it might contains sub-modules 'consumer-loans-frontend' and 'consumer-loans-backend'.";
        }

        public override StrykerInput Type => StrykerInput.ProjectName;

        public ProjectNameInput(string projectName, bool dashboardEnabled)
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
