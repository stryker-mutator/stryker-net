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
        void InitialBuild(bool fullFramework, string path, string solutionPath);
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

        public void InitialBuild(bool fullFramework, string projectPath, string solutionPath)
        {
            _logger.LogInformation("Started initial build using {0}", fullFramework ? "msbuild.exe" : "dotnet build");

            projectPath = Path.GetDirectoryName(projectPath);
            ProcessResult result;
            if (fullFramework)
            {
                if (string.IsNullOrEmpty(solutionPath))
                {
                    throw new StrykerInputException("Stryker could not build your project as no solution file was presented. Please pass the solution path using --solution-path \"..\\my_solution.sln\"");
                }
                solutionPath = Path.GetFullPath(solutionPath);
                string solutionDir = Path.GetDirectoryName(solutionPath);
                var msbuildPath = new MsBuildHelper().GetMsBuildPath(_processExecutor);

                // Build project with MSBuild.exe
                result = _processExecutor.Start(solutionDir, msbuildPath, $"\"{solutionPath}\"");
            }
            else
            {
                // Build with dotnet build
                result = _processExecutor.Start(projectPath, "dotnet", $"build \"{Path.GetFileName(projectPath)}\"");
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
