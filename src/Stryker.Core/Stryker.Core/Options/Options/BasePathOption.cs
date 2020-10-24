using Stryker.Core.Exceptions;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Options
{
    public class BasePathOption : BaseStrykerOption<string>
    {
        static BasePathOption()
        {
        HelpText = string.Empty;
        }

        public override StrykerOption Type => StrykerOption.BasePath;

        public BasePathOption(IFileSystem fileSystem, string basePath)
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
