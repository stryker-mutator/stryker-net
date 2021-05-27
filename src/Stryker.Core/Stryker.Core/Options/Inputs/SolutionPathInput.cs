using Stryker.Core.Exceptions;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Inputs
{
    public class SolutionPathInput : InputDefinition<string>
    {
        public override string Default => null;

        protected override string Description => "Full path to your solution file. Required on dotnet framework.";

        public string Validate(IFileSystem fileSystem)
        {
            if (SuppliedInput is { })
            {
                if (!fileSystem.File.Exists(SuppliedInput))  //validate file existance and maintain moq
                {
                    throw new InputException("Given solution path does not exist: {0}", SuppliedInput);
                }

                return SuppliedInput;
            }
            return null;
        }
    }
}
