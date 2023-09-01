using System;
using System.IO;
using System.IO.Abstractions;
using Spectre.Console;
using Stryker.Core;
using Stryker.Core.Options;

namespace Stryker.CLI.Logging
{
    public interface ILoggingInitializer
    {
        void SetupLogOptions(IStrykerInputs inputs, IFileSystem fileSystem = null);
    }

    public class LoggingInitializer : ILoggingInitializer
    {
        /// <summary>
        /// Creates the needed paths for logging and initializes the logger factory
        /// </summary>
        /// <param name="fileSystem">Mock filesystem</param>
        public void SetupLogOptions(IStrykerInputs inputs, IFileSystem fileSystem = null)
        {
            fileSystem ??= new FileSystem();

            var outputPath = CreateOutputPath(inputs, fileSystem);
            inputs.OutputPathInput.SuppliedInput = outputPath;

            var logLevel = inputs.VerbosityInput.Validate();
            var logToFile = inputs.LogToFileInput.Validate(outputPath);

            ApplicationLogging.ConfigureLogger(logLevel, logToFile, inputs.DevModeInput.Validate(), outputPath);
        }

        private string CreateOutputPath(IStrykerInputs inputs, IFileSystem fileSystem)
        {
            var outputPath = inputs.OutputPathInput.SuppliedInput ?? Path.Combine("StrykerOutput", DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));

            if (!Path.IsPathRooted(outputPath))
            {
                outputPath = Path.Combine(inputs.BasePathInput.SuppliedInput, outputPath);
            }

            // outputpath should always be created
            fileSystem.Directory.CreateDirectory(FilePathUtils.NormalizePathSeparators(outputPath));

            // add gitignore if it didn't exist yet
            var gitignorePath = FilePathUtils.NormalizePathSeparators(Path.Combine(outputPath, ".gitignore"));
            if (!fileSystem.File.Exists(gitignorePath))
            {
                try
                {
                    fileSystem.File.WriteAllText(gitignorePath, "*");
                }
                catch (IOException e)
                {
                    AnsiConsole.WriteLine($"Could't create gitignore file because of error {e.Message}. \n" +
                        "If you use any diff compare features this may mean that stryker logs show up as changes.");
                }
            }

            return outputPath;
        }
    }
}
