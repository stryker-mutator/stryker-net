using Stryker.Core.Exceptions;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Inputs
{
    public class BasePathInput : SimpleStrykerInput<string>
    {
        static BasePathInput()
        {
            HelpText = string.Empty;
        }

        public override StrykerInput Type => StrykerInput.BasePath;

        public BasePathInput(IFileSystem fileSystem, string basePath)
        {
            if (basePath.IsNullOrEmptyInput())
            {
                throw new StrykerInputException("Base path cannot be null.");
            }

            if (!fileSystem.Directory.Exists(Value))  // validate base path is valid path
            {
                throw new StrykerInputException("Base path does not exist.");
            }

            Value = basePath;
        }
    }
}
