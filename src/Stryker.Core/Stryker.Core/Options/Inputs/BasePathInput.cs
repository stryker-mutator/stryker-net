using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class BasePathInput : OptionDefinition<string>
    {
        protected override string Description => "The path from which stryker is started.";

        public BasePathInput(IFileSystem fileSystem, string basePath)
        {
            if (basePath.IsNullOrEmptyInput())
            {
                throw new StrykerInputException("Base path cannot be null.");
            }

            if (!fileSystem.Directory.Exists(basePath)) // validate base path is valid path
            {
                throw new StrykerInputException("Base path must exist.");
            }

            Value = basePath;
        }
    }
}
