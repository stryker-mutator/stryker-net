using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectVersionInput : OptionDefinition<string>
    {

        protected override string Description => "Project version used in reporters.";

        public ProjectVersionInput() { }
        public ProjectVersionInput(string projectVersion, string fallbackVersion, bool dashboardEnabled, bool dashboardCompareEnabled)
        {
            if (dashboardEnabled)
            {
                if (projectVersion.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("When the stryker dashboard is enabled the project version is required. Please provide a project version.");
                }

                if (dashboardCompareEnabled && fallbackVersion == projectVersion)
                {
                    throw new StrykerInputException("Project version cannot be the same as the fallback version. Please provide a different version for one of them.");
                }

                Value = projectVersion;
            }
        }
    }
}
