using Stryker.Abstractions.Exceptions;

namespace Stryker.Abstractions.Options.Inputs
{
    public class SourceProjectNameInput : Input<string>
    {
        public override string Default => string.Empty;

        protected override string Description => @"Used to find the project to test in the project references of the test project. Example: ""ExampleProject.csproj""";

        public string Validate()
        {
            if (SuppliedInput is not null)
            {
                if (string.IsNullOrWhiteSpace(SuppliedInput))
                {
                    throw new InputException("Project file cannot be empty.");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
