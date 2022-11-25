using Stryker.Core.Exceptions;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Inputs
{
    public class SolutionInput : Input<string>
    {
        public override string Default => null;

        protected override string Description => "Full path to your solution file. Required on dotnet framework.";

        public string Validate(IFileSystem fileSystem)
        {
            if (SuppliedInput is not null)
            {
                if(!SuppliedInput.EndsWith(".sln"))
                {
                    throw new InputException($"Given path is not a solution file: {SuppliedInput}");
                }
                var fullPath = fileSystem.Path.GetFullPath(SuppliedInput);
                if (!fileSystem.File.Exists(fullPath))
                {
                    throw new InputException($"Given path does not exist: {SuppliedInput}");
                }

                return fullPath;
            }
            return null;
        }
    }
}
