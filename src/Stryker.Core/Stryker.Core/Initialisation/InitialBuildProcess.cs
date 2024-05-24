using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System;
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
            _logger.LogDebug("Started initial build using {BuildExecutable}", fullFramework ? "msbuild.exe" : "dotnet build");

        ProcessResult result;
        string buildCommand;
        string options;
        if (fullFramework)
        {
            if (string.IsNullOrEmpty(solutionPath))
            {
                throw new InputException("Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
            }
            result = BuildSolutionWithMsBuild(configuration, ref solutionPath, ref msbuildPath, out options);
            buildCommand = msbuildPath;
        }
        else
        {
            var buildPath = QuotesIfNeeded(!string.IsNullOrEmpty(solutionPath) ? solutionPath : Path.GetFileName(projectPath));
            buildCommand = "dotnet";

            _logger.LogDebug("Initial build using path: {buildPath}", buildPath);
            if (!string.IsNullOrEmpty(configuration))
            {
                options = $"build {buildPath} -c {QuotesIfNeeded(configuration)}";
            }
            else
            {
                options = $"build {buildPath}";
            }
            // Build with dotnet build
            result = _processExecutor.Start(Path.GetDirectoryName(projectPath), buildCommand, options);
            if (result.ExitCode!=ExitCodes.Success && !string.IsNullOrEmpty(solutionPath))
            {
                // dump previous build result
                _logger.LogTrace("Initial build output: {0}", result.Output);
                _logger.LogWarning("Dotnet build failed, trying with MsBuild.");
                buildCommand = msbuildPath;
                result = BuildSolutionWithMsBuild(configuration, ref solutionPath, ref msbuildPath, out options);
            }

        }
        CheckBuildResult(result, buildCommand, options);
    }

    private ProcessResult BuildSolutionWithMsBuild(string configuration, ref string solutionPath,
        ref string msbuildPath, out string options)
    {
        solutionPath = Path.GetFullPath(solutionPath);
        var solutionDir = Path.GetDirectoryName(solutionPath);
        solutionPath = QuotesIfNeeded(solutionPath);
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            msbuildPath ??= new MsBuildHelper().GetMsBuildPath(_processExecutor);
            options = solutionPath;
        }
        else
        {
            msbuildPath = "dotnet";
            options = $"msbuild {solutionPath}";
        }
        if (!string.IsNullOrEmpty(configuration))
        {
            options += $" /property:Configuration={QuotesIfNeeded(configuration)}";
        }
        // Build project with MSBuild.exe
        var result = _processExecutor.Start(solutionDir, msbuildPath, options);
        if (result.ExitCode != ExitCodes.Success)
        {
            _logger.LogWarning("MsBuild failed to build the solution, trying to restore packages and build again.");
            _processExecutor.Start(solutionDir, msbuildPath, $"${options} -t:restore -p:RestorePackagesConfig=true");
            result = _processExecutor.Start(solutionDir, msbuildPath, solutionPath);
        }
        return result;
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
        if (!parameter.Contains(' '))
        {
            return parameter;
        }
        return $"\"{parameter}\"";
    }
}
