using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectUnderTestNameInput : InputDefinition<string>
    {
        public override string Default => string.Empty;

        protected override string Description => @"Used to find the project to test in the project references of the test project. Example: ""ExampleProject.csproj""";

        public string Validate()
        {
            if (SuppliedInput is { })
            {
                if (SuppliedInput.IsNullOrEmptyInput())
                {
                    throw new InputException("Project under test name filter cannot be empty.");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
