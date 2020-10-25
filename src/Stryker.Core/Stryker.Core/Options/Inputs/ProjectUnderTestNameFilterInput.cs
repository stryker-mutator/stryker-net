using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectUnderTestNameFilterInput : SimpleStrykerInput<string>
    {
        static ProjectUnderTestNameFilterInput()
        {
            HelpText = @"Used for matching the project references when finding the project to mutate. Example: ""ExampleProject.csproj""";
        }

        public override StrykerInput Type => StrykerInput.ProjectUnderTestName;

        public ProjectUnderTestNameFilterInput(string projectUnderTestNameFilter)
        {
            if (projectUnderTestNameFilter is { })
            {
                if (projectUnderTestNameFilter.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("Project under test name filter cannot be empty.");
                }

                Value = projectUnderTestNameFilter;
            }
        }
    }
}
