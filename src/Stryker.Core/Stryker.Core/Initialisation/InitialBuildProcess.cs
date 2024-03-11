using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System.IO;

namespace Stryker.Core.Initialisation
{
    public interface IInitialBuildProcess
    {
        void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string msbuildPath = null);
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

        public void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string msbuildPath = null)
        {
            _logger.LogDebug("Started initial build using {0}", fullFramework ? "msbuild.exe" : "dotnet build");

            ProcessResult result;
            string buildCommand;
            string buildPath;
            if (fullFramework)
            {
                if (string.IsNullOrEmpty(solutionPath))
                {
                    throw new InputException("Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
                }
                result = BuildSolutionWithMsBuild(ref solutionPath, ref msbuildPath);
                buildCommand = msbuildPath;
                buildPath = solutionPath;
            }
            else
            {
                buildPath = !string.IsNullOrEmpty(solutionPath) ? solutionPath : Path.GetFileName(projectPath);
                buildCommand = "dotnet build";

                _logger.LogDebug("Initial build using path: {buildPath}", buildPath);
                // Build with dotnet build
                result = _processExecutor.Start(Path.GetDirectoryName(projectPath), "dotnet", $"build \"{buildPath}\"");
                if (result.ExitCode!=ExitCodes.Success && !string.IsNullOrEmpty(solutionPath))
                {
                    _logger.LogWarning("Dotnet build failed, trying with MsBuild.");
                    buildCommand = msbuildPath;
                    result = BuildSolutionWithMsBuild(ref solutionPath, ref msbuildPath);
                }

            }
            CheckBuildResult(result, buildCommand, $"\"{buildPath}\"");
        }

        private ProcessResult BuildSolutionWithMsBuild(ref string solutionPath, ref string msbuildPath)
        {
            solutionPath = Path.GetFullPath(solutionPath);
            var solutionDir = Path.GetDirectoryName(solutionPath);
            msbuildPath ??= new MsBuildHelper().GetMsBuildPath(_processExecutor);

            // Build project with MSBuild.exe
            var result = _processExecutor.Start(solutionDir, msbuildPath, $"\"{solutionPath}\"");
            if (result.ExitCode != ExitCodes.Success)
            {
                _logger.LogWarning("MsBuild failed to build the solution, trying to restore packages and build again.");
                _processExecutor.Start(solutionDir, msbuildPath, $"\"{solutionPath}\" -t:restore -p:RestorePackagesConfig=true");
                result = _processExecutor.Start(solutionDir, msbuildPath, $"\"{solutionPath}\"");
            }
            return result;
        }

        private void CheckBuildResult(ProcessResult result, string buildCommand, string buildPath)
        {
            if (result.ExitCode != ExitCodes.Success)
            {
                _logger.LogError("Initial build failed: {0}", result.Output);
                // Initial build failed
                throw new InputException(result.Output, FormatBuildResultErrorString(buildCommand, buildPath));
            }
            _logger.LogTrace("Initial build output {0}", result.Output);
            _logger.LogDebug("Initial build successful");
        }

        private static string FormatBuildResultErrorString(string buildCommand, string buildPath)
        {
            if (Path.IsPathRooted(buildCommand))
            {
                buildCommand = $"\"{buildCommand}\"";
            }

            return "Initial build of targeted project failed. Please make sure the targeted project is buildable." +
                   $" You can reproduce this error yourself using: \"{buildCommand} {buildPath}\"";
        }
    }
}
