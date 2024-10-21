using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Logging;
using Stryker.Configuration;
using Stryker.Core.Helpers;
using Stryker.Core.Testing;

namespace Stryker.Core.Initialisation;

public interface INugetRestoreProcess
{
    void RestorePackages(string solutionPath, string msbuildPath = null);
}

/// <summary>
/// Restores nuget packages for a given solution file
/// </summary>
public class NugetRestoreProcess : INugetRestoreProcess
{
    private IProcessExecutor ProcessExecutor { get; set; }
    private readonly ILogger _logger;

    public NugetRestoreProcess(IProcessExecutor processExecutor = null)
    {
        ProcessExecutor = processExecutor ?? new ProcessExecutor();
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<NugetRestoreProcess>();
    }

    public void RestorePackages(string solutionPath, string msbuildPath = null)
    {
        _logger.LogInformation("Restoring nuget packages using nuget.exe");
        if (string.IsNullOrWhiteSpace(solutionPath))
        {
            throw new InputException(
                "Solution path is required on .net framework projects. Please supply the solution path.");
        }

        solutionPath = Path.GetFullPath(solutionPath);
        var solutionDir = Path.GetDirectoryName(solutionPath);

        var helper = new MsBuildHelper(null, ProcessExecutor, msbuildPath, _logger);
        var msBuildVersion = FindMsBuildShortVersion(helper);

        // Validate nuget.exe is installed and included in path
        var nugetWhereExeResult = ProcessExecutor.Start(solutionDir, "where.exe", "nuget.exe");
        if (!nugetWhereExeResult.Output.ToLowerInvariant().Contains("nuget.exe"))
        {
            // try to extend the search
            nugetWhereExeResult = ProcessExecutor.Start(solutionDir, "where.exe",
                $"/R {Path.GetPathRoot(msbuildPath)} nuget.exe");

            if (!nugetWhereExeResult.Output.ToLowerInvariant().Contains("nuget.exe"))
                throw new InputException(
                    "Nuget.exe should be installed to restore .net framework nuget packages. Install nuget.exe and make sure it's included in your path.");
        }

        // Get the first nuget.exe path from the where.exe output
        var nugetPath = nugetWhereExeResult.Output
            .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First().Trim();

        // Restore packages using nuget.exe
        var nugetRestoreCommand = $"restore \"{solutionPath}\"";
        if (!string.IsNullOrEmpty(msBuildVersion))
        {
            nugetRestoreCommand += $" -MsBuildVersion \"{msBuildVersion}\"";
        }

        _logger.LogDebug("Restoring packages using command: {NugetPath} {NugetRestoreCommand}", nugetPath,
            nugetRestoreCommand);

        const int NugetRestoreTimeoutMs = 120000;
        try
        {
            var nugetRestoreResult = ProcessExecutor.Start(Path.GetDirectoryName(nugetPath), nugetPath,
                nugetRestoreCommand, timeoutMs: NugetRestoreTimeoutMs);
            if (nugetRestoreResult.ExitCode != ExitCodes.Success)
            {
                _logger.LogError("Failed to restore nuget packages. Nuget error: {Error}",
                    nugetRestoreResult.Error);
                return;
            }

            _logger.LogDebug("Restored packages using nuget.exe, output: {Error}", nugetRestoreResult.Output);
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("Failed to restore nuget packages in less than {time} seconds.", NugetRestoreTimeoutMs/1000);
        }
    }

    private string FindMsBuildShortVersion(MsBuildHelper helper)
    {
        var msBuildVersionOutput = helper.GetVersion();
        string msBuildVersion;
        if (string.IsNullOrWhiteSpace(msBuildVersionOutput))
        {
            msBuildVersion = string.Empty;
            _logger.LogInformation("Auto detected msbuild at: {MsBuildPath}, but failed to get version.", helper.GetMsBuildPath());
        }
        else
        {
            msBuildVersion = msBuildVersionOutput.Trim();
            _logger.LogDebug("Auto detected msbuild version {MsBuildVersion} at: {MsBuildPath}", msBuildVersion,
                 helper.GetMsBuildPath());
        }

        return msBuildVersion;
    }
}
