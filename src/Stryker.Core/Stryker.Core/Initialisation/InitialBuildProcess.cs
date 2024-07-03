using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System.IO;

namespace Stryker.Core.Initialisation;

public interface IInitialBuildProcess
{
    void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string configuration = null,
        string msbuildPath = null);
}

public class InitialBuildProcess : IInitialBuildProcess
{
    private readonly IProcessExecutor _processExecutor;
    private readonly ILogger _logger;

    public InitialBuildProcess(IProcessExecutor processExecutor = null)
    {
        _processExecutor = processExecutor ?? new ProcessExecutor();
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialBuildProcess>();
    }

    public void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string configuration=null,
        string msbuildPath = null)
    {
        if (fullFramework && string.IsNullOrEmpty(solutionPath))
        {
            throw new InputException("Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
        }

        var msBuildHelper = new MsBuildHelper(executor: _processExecutor ,msBuildPath: msbuildPath);

        _logger.LogDebug("Started initial build using dotnet build");

        var target = !string.IsNullOrEmpty(solutionPath) ? solutionPath : Path.GetFileName(projectPath);
        var buildPath = QuotesIfNeeded(target);
        var (result, exe, args) = msBuildHelper.BuildProject(Path.GetDirectoryName(target), buildPath, fullFramework, configuration);

        if (result.ExitCode!=ExitCodes.Success && !string.IsNullOrEmpty(solutionPath))
        {
            // dump previous build result
            _logger.LogTrace("Initial build output: {0}", result.Output);
            _logger.LogWarning("Dotnet build failed, trying with MsBuild and forcing package restore.");
            (result, exe, args) = msBuildHelper.BuildProject(Path.GetDirectoryName(target),
                buildPath,
                true,
                configuration,
                "-t:restore -p:RestorePackagesConfig=true");
            if (result.ExitCode != ExitCodes.Success)
            {
                _logger.LogWarning("Package restore failed: {Result}", result.Output);
            }
            _logger.LogTrace("Last attempt to build.", result.Output);
            (result, exe, args) = msBuildHelper.BuildProject(Path.GetDirectoryName(target),
                buildPath,
                true,
                configuration);
        }

        CheckBuildResult(result, exe, args);
    }

    private void CheckBuildResult(ProcessResult result, string buildCommand, string options)
    {
        if (result.ExitCode != ExitCodes.Success)
        {
            _logger.LogError("Initial build failed: {Result}", result.Output);
            // Initial build failed
            throw new InputException(result.Output, FormatBuildResultErrorString(buildCommand, options));
        }
        _logger.LogTrace("Initial build output {Result}", result.Output);
        _logger.LogDebug("Initial build successful");
    }

    private static string FormatBuildResultErrorString(string buildCommand, string options) =>
        "Initial build of targeted project failed. Please make sure the targeted project is buildable." +
        $" You can reproduce this error yourself using: \"{QuotesIfNeeded(buildCommand)} {options}\"";

    private static string QuotesIfNeeded(string parameter)
    {
        if (!parameter.Contains(' ') || parameter.Length<3 || (parameter[0] == '"' && parameter[^1]=='"'))
        {
            return parameter;
        }
        return $"\"{parameter}\"";
    }
}
