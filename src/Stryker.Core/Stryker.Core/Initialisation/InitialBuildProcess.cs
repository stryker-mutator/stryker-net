using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration;
using Stryker.Core.Helpers;
using Stryker.Core.Helpers.ProcessUtil;

namespace Stryker.Core.Initialisation;

public interface IInitialBuildProcess
{
    void InitialBuild(bool fullFramework,
        string projectPath,
        string solutionPath,
        string configuration = null,
        string platform = null,
        string targetFramework = null,
        string msbuildPath = null);
}

public class InitialBuildProcess : IInitialBuildProcess
{
    private readonly IProcessExecutor _processExecutor;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public InitialBuildProcess(
        IProcessExecutor processExecutor,
        IFileSystem fileSystem,
        ILogger<InitialBuildProcess> logger)
    {
        _processExecutor = processExecutor ?? throw new ArgumentNullException(nameof(processExecutor));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string configuration = null,
        string platform = null, string targetFramework = null,
        string msbuildPath = null)
    {
        if (fullFramework)
        {
            // ensure prerequisites for building .NETFramework projects are met
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                throw new InputException("Stryker cannot build .NET Framework projects on non-Windows platforms.");
            }
            if (string.IsNullOrEmpty(solutionPath))
            {
                throw new InputException("Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
            }
        }

        var msBuildHelper = new MsBuildHelper(fileSystem: _fileSystem, executor: _processExecutor, msBuildPath: msbuildPath);

        _logger.LogDebug("Started initial build using dotnet build");

        var target = !string.IsNullOrEmpty(solutionPath) ? solutionPath : projectPath;
        var buildPath = _fileSystem.Path.GetFileName(target);
        var directoryName = _fileSystem.Path.GetDirectoryName(target);
        var (result, exe, args) = msBuildHelper.BuildProject(directoryName,
            buildPath,
            fullFramework,
            configuration: configuration,
            platform: platform,
            forcedFramework: targetFramework);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && result.ExitCode != ExitCodes.Success && !string.IsNullOrEmpty(solutionPath))
        {
            // dump previous build result
            _logger.LogTrace("Initial build output: {0}", result.Output);
            _logger.LogWarning("Dotnet build failed, trying with MsBuild and forcing package restore.");
            (result, _, _) = msBuildHelper.BuildProject(directoryName,
                buildPath,
                true,
                configuration,
                options: "-t:restore -p:RestorePackagesConfig=true", forcedFramework: targetFramework);

            if (result.ExitCode != ExitCodes.Success)
            {
                _logger.LogWarning("Package restore failed: {Result}", result.Output);
            }
            _logger.LogTrace("Last attempt to build.");
            (result, exe, args) = msBuildHelper.BuildProject(directoryName,
                buildPath,
                true,
                configuration,
                forcedFramework: targetFramework);
        }

        CheckBuildResult(result, target, exe, args);
    }

    private void CheckBuildResult(ProcessResult result, string path, string buildCommand, string options)
    {
        if (result.ExitCode != ExitCodes.Success)
        {
            _logger.LogError("Initial build failed. Command was [{exe} {args}] (in folder '{folder}'). Result: {Result}", buildCommand, options, path, result.Output);
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
        if (!parameter.Contains(' ') || parameter.Length < 3 || parameter[0] == '"' && parameter[^1] == '"')
        {
            return parameter;
        }
        return $"\"{parameter}\"";
    }
}
