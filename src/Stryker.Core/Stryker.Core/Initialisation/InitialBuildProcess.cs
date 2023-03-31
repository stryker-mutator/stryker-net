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
            if (fullFramework)
            {
                if (string.IsNullOrEmpty(solutionPath))
                {
                    throw new InputException("Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
                }
                solutionPath = Path.GetFullPath(solutionPath);
                var solutionDir = Path.GetDirectoryName(solutionPath);
                msbuildPath ??= new MsBuildHelper().GetMsBuildPath(_processExecutor);

                // Build project with MSBuild.exe
                result = _processExecutor.Start(solutionDir, msbuildPath, $"\"{solutionPath}\"");
                CheckBuildResult(result, msbuildPath, $"\"{solutionPath}\"");
            }
            else
            {
                var buildPath = !string.IsNullOrEmpty(solutionPath) ? solutionPath : Path.GetFileName(projectPath);

                _logger.LogDebug("Initial build using path: {buildPath}", buildPath);
                // Build with dotnet build
                result = _processExecutor.Start(projectPath, "dotnet", $"build \"{buildPath}\"");

                CheckBuildResult(result, "dotnet build", $"\"{projectPath}\"");
            }
        }

        private void CheckBuildResult(ProcessResult result, string buildCommand, string buildPath)
        {
            _logger.LogTrace("Initial build output {0}", result.Output);
            if (result.ExitCode != ExitCodes.Success)
            {
                // Initial build failed
                throw new InputException(result.Output, FormatBuildResultErrorString(buildCommand, buildPath));
            }
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
