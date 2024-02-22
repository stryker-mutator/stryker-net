using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;

namespace Stryker.Core.Initialisation
{
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
            _logger.LogInformation("Restoring nuget packages using {0}", "nuget.exe");
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                throw new InputException("Solution path is required on .net framework projects. Please supply the solution path.");
            }
            solutionPath = Path.GetFullPath(solutionPath);
            var solutionDir = Path.GetDirectoryName(solutionPath);


            // Locate MSBuild.exe
            msbuildPath ??= new MsBuildHelper().GetMsBuildPath(ProcessExecutor);
            var msBuildVersionOutput = ProcessExecutor.Start(solutionDir, msbuildPath, "-version /nologo");
            string msBuildVersion;
            if (msBuildVersionOutput.ExitCode != ExitCodes.Success)
            {
                msBuildVersion = string.Empty;
                _logger.LogDebug("Auto detected msbuild at: {0}, but failed to get version.", msbuildPath);
            }
            else
            {
                msBuildVersion = msBuildVersionOutput.Output.Trim();
                _logger.LogDebug("Auto detected msbuild version {0} at: {1}", msBuildVersion, msbuildPath);
            }

            // Validate nuget.exe is installed and included in path
            var nugetWhereExeResult = ProcessExecutor.Start(solutionDir, "where.exe", "nuget.exe");
            if (!nugetWhereExeResult.Output.ToLowerInvariant().Contains("nuget.exe"))
            {
                // try to extend the search
                nugetWhereExeResult = ProcessExecutor.Start(solutionDir, "where.exe", $"/R {Path.GetPathRoot(msbuildPath)} nuget.exe");

                if (!nugetWhereExeResult.Output.ToLowerInvariant().Contains("nuget.exe"))
                    throw new InputException("Nuget.exe should be installed to restore .net framework nuget packages. Install nuget.exe and make sure it's included in your path.");
            }
            // Get the first nuget.exe path from the where.exe output
            var nugetPath = nugetWhereExeResult.Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First().Trim();


            // Restore packages using nuget.exe
            var nugetRestoreCommand = $"restore \"{solutionPath}\"";
            if (!string.IsNullOrEmpty(msBuildVersion))
            {
                nugetRestoreCommand += $" -MsBuildVersion \"{msBuildVersion}\"";
            }
            _logger.LogDebug("Restoring packages using command: {0} {1}", nugetPath, nugetRestoreCommand);

            try
            {
                var nugetRestoreResult = ProcessExecutor.Start(Path.GetDirectoryName(nugetPath), nugetPath, nugetRestoreCommand, timeoutMs: 120000);
                if (nugetRestoreResult.ExitCode != ExitCodes.Success)
                {
                    _logger.LogCritical($"Failed to restore nuget packages. Nuget error: {nugetRestoreResult.Error}");
                    throw new InputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.", nugetRestoreResult.Output);
                }
                _logger.LogDebug("Restored packages using nuget.exe, output: {0}", nugetRestoreResult.Output);
            }
            catch (OperationCanceledException)
            {
                throw new InputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.");
            }
        }
    }
}
