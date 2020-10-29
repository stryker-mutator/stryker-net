using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class BasePathInput : SimpleStrykerInput<string>
    {
        public override StrykerInput Type => StrykerInput.BasePath;

        protected override string Description => "The path from which stryker is started.";

        public BasePathInput(IFileSystem fileSystem, string basePath)
        {
            if (basePath.IsNullOrEmptyInput())
            {
                throw new StrykerInputException("Base path cannot be null.");
            }

            if (!fileSystem.Directory.Exists(Value))  // validate base path is valid path
            {
                throw new StrykerInputException("Base path must exist.");
            }

            Value = basePath;
        }
    }
}
