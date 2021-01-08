using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectUnderTestNameInput : OptionDefinition<string>
    {
        public override StrykerOption Type => StrykerOption.ProjectUnderTestName;

        protected override string Description => @"Used to find the project to test in the project references of the test project. Example: ""ExampleProject.csproj""";

        public ProjectUnderTestNameInput() { }
        public ProjectUnderTestNameInput(string projectUnderTestNameFilter)
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
