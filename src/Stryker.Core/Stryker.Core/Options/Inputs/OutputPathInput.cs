using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using System;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Inputs
{
    public class OutputPathInput : OptionDefinition<string>
    {
        protected override string Description => string.Empty;

        public OutputPathInput(ILogger logger, IFileSystem fileSystem, string basepath)
        {
            if (basepath.IsNullOrEmptyInput())
            {
                throw new StrykerInputException("The output path cannot be empty.");
            }

            var outputPath = Path.Combine(basepath, "StrykerOutput", DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));
            fileSystem.Directory.CreateDirectory(FilePathUtils.NormalizePathSeparators(outputPath));

            // Create output dir with gitignore
            var gitignorePath = FilePathUtils.NormalizePathSeparators(Path.Combine(basepath, "StrykerOutput", ".gitignore"));

            if (!fileSystem.File.Exists(gitignorePath))
            {
                try
                {
                    fileSystem.File.WriteAllText(gitignorePath, "*");
                }
                catch (IOException e)
                {
                    logger.LogWarning("Could't create gitignore file because of error {error}. \n" +
                        "If you use any diff compare features this may mean that stryker logs show up as changes.", e.Message);
                }
            }
            Value = outputPath;
        }
    }
}
