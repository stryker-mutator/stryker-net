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
            _logger.LogDebug("Started initial build using {0}", fullFramework ? "msbuild.exe" : "dotnet build");

            ProcessResult result;
            string buildCommand;
            string buildPath;
            string options;
            if (fullFramework)
            {
                if (string.IsNullOrEmpty(solutionPath))
                {
                    throw new InputException("Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
                }
                result = BuildSolutionWithMsBuild(configuration, ref solutionPath, ref msbuildPath, out options);
                buildCommand = msbuildPath;
                buildPath = solutionPath;
            }
            else
            {
                buildPath = QuotesIfNeeded(!string.IsNullOrEmpty(solutionPath) ? solutionPath : Path.GetFileName(projectPath));
                buildCommand = "dotnet";

                _logger.LogDebug("Initial build using path: {buildPath}", buildPath);
                options = "build ";
                if (!string.IsNullOrEmpty(configuration))
                {
                    options = $"build -c {QuotesIfNeeded(configuration)} ";
                }
                // Build with dotnet build
                result = _processExecutor.Start(Path.GetDirectoryName(projectPath), buildCommand, options+buildPath);
                if (result.ExitCode!=ExitCodes.Success && !string.IsNullOrEmpty(solutionPath))
                {
                    _logger.LogWarning("Dotnet build failed, trying with MsBuild.");
                    buildCommand = msbuildPath;
                    result = BuildSolutionWithMsBuild(configuration, ref solutionPath, ref msbuildPath, out options);
                }

            }
            CheckBuildResult(result, buildCommand, options, buildPath);
        }

        private ProcessResult BuildSolutionWithMsBuild(string configuration, ref string solutionPath,
            ref string msbuildPath, out string options)
        {
            solutionPath = Path.GetFullPath(solutionPath);
            var solutionDir = Path.GetDirectoryName(solutionPath);
            solutionPath = QuotesIfNeeded(solutionPath);
            msbuildPath ??= new MsBuildHelper().GetMsBuildPath(_processExecutor);
            options = string.Empty;
            if (!string.IsNullOrEmpty(configuration))
            {
                options = $" /property:Configuration={QuotesIfNeeded(configuration)} ";
            }
            // Build project with MSBuild.exe
            var result = _processExecutor.Start(solutionDir, msbuildPath, $"{solutionPath}{options}");
            if (result.ExitCode != ExitCodes.Success)
            {
                _logger.LogWarning("MsBuild failed to build the solution, trying to restore packages and build again.");
                _processExecutor.Start(solutionDir, msbuildPath, $"{solutionPath}${options}-t:restore -p:RestorePackagesConfig=true");
                result = _processExecutor.Start(solutionDir, msbuildPath, solutionPath);
            }
            return result;
        }

        private void CheckBuildResult(ProcessResult result, string buildCommand, string options, string buildPath)
        {
            if (result.ExitCode != ExitCodes.Success)
            {
                _logger.LogError("Initial build failed: {0}", result.Output);
                // Initial build failed
                throw new InputException(result.Output, FormatBuildResultErrorString(buildCommand, options, buildPath));
            }
            _logger.LogTrace("Initial build output {0}", result.Output);
            _logger.LogDebug("Initial build successful");
        }

        private static string FormatBuildResultErrorString(string buildCommand, string options, string buildPath) =>
            "Initial build of targeted project failed. Please make sure the targeted project is buildable." +
            $" You can reproduce this error yourself using: \"{QuotesIfNeeded(buildCommand)} {options}{buildPath}\"";

        private static string QuotesIfNeeded(string parameter)
        {
            if (!parameter.Contains(' '))
            {
                return parameter;
            }
            return $"\"{parameter}\"";
        }
    }
}
