using System.IO;
using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class BasePathInput : OptionDefinition<string>
    {
        protected override string Description => "The path from which stryker is started.";

        public override string Default => Directory.GetCurrentDirectory();

        public string Validate(IFileSystem fileSystem)
        {
            if (SuppliedInput.IsNullOrEmptyInput())
            {
                throw new StrykerInputException("Base path cannot be null.");
            }

            if (!fileSystem.Directory.Exists(SuppliedInput)) // validate base path is valid path
            {
                throw new StrykerInputException("Base path must exist.");
            }

            return SuppliedInput;
        }
    }
}
