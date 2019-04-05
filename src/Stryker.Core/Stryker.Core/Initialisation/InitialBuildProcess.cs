using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System;
using System.IO;

namespace Stryker.Core.Initialisation
{
    public interface IInitialBuildProcess
    {
        void InitialBuild(bool fullFramework, string path, string solutionPath, string projectName);
    }

    public class InitialBuildProcess : IInitialBuildProcess
    {
        private IProcessExecutor _processExecutor { get; set; }
        private ILogger _logger { get; set; }

        public InitialBuildProcess(IProcessExecutor processExecutor = null)
        {
            _processExecutor = processExecutor ?? new ProcessExecutor();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialBuildProcess>();
        }

        public void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string projectName)
        {
            _logger.LogInformation("Started initial build using {0}", fullFramework ? "msbuild.exe" : "dotnet build");
            ProcessResult result = null;
            if (fullFramework)
            {
                if (string.IsNullOrWhiteSpace(solutionPath))
                {
                    throw new StrykerInputException("Solution path is required on .net framework projects. Please provide your solution path using --solution-path ...");
                }
                solutionPath = Path.GetFullPath(solutionPath);
                string solutionDir = Path.GetDirectoryName(solutionPath);

                _logger.LogDebug("Searching for nuget.exe executable path");
                // Validate nuget.exe is installed and included in path
                var nugetWhereExeResult = _processExecutor.Start(solutionDir, "where.exe", "nuget.exe");
                if (!nugetWhereExeResult.Output.Contains("nuget.exe"))
                {
                    throw new StrykerInputException("Nuget.exe should be installed to restore .net framework nuget packages. Install nuget.exe and make sure it's included in your path.");
                }
                var nugetExePath = nugetWhereExeResult.Output.Trim();
                _logger.LogDebug($"Nuget.exe executable path found at {nugetExePath}");

                _logger.LogDebug("Searching for msbuild.exe executable path");
                // Locate MSBuild.exe
                var msBuildPath = new MsBuildHelper().GetMsBuildPath(_processExecutor);

                var msBuildVersionOutput = _processExecutor.Start(solutionDir, msBuildPath, "-version /nologo");
                if (msBuildVersionOutput.ExitCode != 0)
                {
                    _logger.LogError("Unable to detect msbuild version");
                }
                _logger.LogInformation("Auto detected msbuild version {0} at: {1}", msBuildVersionOutput.Output.Trim(), msBuildPath);

                // Restore packages using nuget.exe
                var nugetRestoreCommand = string.Format("restore \"{0}\" -MsBuildVersion \"{1}\"", solutionPath, msBuildVersionOutput.Output.Trim());
                _logger.LogDebug("Restoring packages using command: {0} {1}", nugetExePath, nugetRestoreCommand);

                try
                {
                    var nugetRestoreResult = _processExecutor.Start(solutionDir, nugetExePath, nugetRestoreCommand, timeoutMS: 120000);
                    if (nugetRestoreResult.ExitCode != 0)
                    {
                        throw new StrykerInputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.", nugetRestoreResult.Output);
                    }
                    _logger.LogDebug("Restored packages using nuget.exe, output: {0}", nugetRestoreResult.Output);
                }
                catch (OperationCanceledException)
                {
                    throw new StrykerInputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.");
                }

                // Build project with MSBuild.exe
                result = _processExecutor.Start(solutionDir, msBuildPath, $"\"{solutionPath}\"");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(solutionPath))
                {
                    _logger.LogWarning("Stryker is running on a .net core project but a solution path was provided. The solution path option is only needed on .net framework projects and can be removed. Please update your stryker options.");
                }
                // Build with dotnet build
                result = _processExecutor.Start(projectPath, "dotnet", $"build \"{projectName}\"");
            }

            _logger.LogDebug("Initial build output {0}", result.Output);
            if (result.ExitCode != 0)
            {
                // Initial build failed
                throw new StrykerInputException(result.Output, "Initial build of targeted project failed. Please make targeted project buildable. See above message for build output.");
            }
            _logger.LogInformation("Initial build successful");
        }
    }
}
