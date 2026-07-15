using System;
using System.IO;
using System.IO.Abstractions;
using Serilog.Events;
using Spectre.Console;
using Stryker.Abstractions.Options;
using Stryker.Configuration.Options;
using Stryker.Utilities;

namespace Stryker.CLI.Logging;

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

        var diagnoseMode = inputs.DiagModeInput.Validate();
        var logLevel = inputs.VerbosityInput.Validate();
        if (diagnoseMode && logLevel>LogEventLevel.Debug)
        {
            logLevel = LogEventLevel.Debug;
        }
        var logToFile = inputs.LogToFileInput.Validate(outputPath) || diagnoseMode;

        ApplicationLogging.ConfigureLogger(logLevel, logToFile, diagnoseMode, outputPath);
    }

    private static string CreateOutputPath(IStrykerInputs inputs, IFileSystem fileSystem)
    {
        // The stable output root. When no output path is supplied the per-run output lives in a
        // timestamped subfolder of this root; an explicitly supplied output path is the root itself.
        // The root is where the disk baseline is stored (so it can be found on the next run) and
        // where the gitignore is placed (so the baseline and all run outputs are excluded from git).
        var outputRoot = inputs.OutputPathInput.SuppliedInput ?? "StrykerOutput";
        var outputPath = inputs.OutputPathInput.SuppliedInput ?? Path.Combine(outputRoot, DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));

        if (!Path.IsPathRooted(outputRoot))
        {
            outputRoot = Path.Combine(inputs.BasePathInput.SuppliedInput, outputRoot);
        }

        if (!Path.IsPathRooted(outputPath))
        {
            outputPath = Path.Combine(inputs.BasePathInput.SuppliedInput, outputPath);
        }

        // outputpath should always be created
        fileSystem.Directory.CreateDirectory(FilePathUtils.NormalizePathSeparators(outputPath));

        // store the baseline under the stable output root so it follows --output and persists across runs
        inputs.BaselineOutputInput.SuppliedInput = outputRoot;

        // add gitignore to the output root if it didn't exist yet
        var gitignorePath = FilePathUtils.NormalizePathSeparators(Path.Combine(outputRoot, ".gitignore"));
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
