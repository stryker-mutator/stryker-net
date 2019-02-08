using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System;

namespace Stryker.Core.Initialisation
{
    public interface IInitialBuildProcess
    {
        void InitialBuild(bool fullFramework, string path, string projectName);
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

        public void InitialBuild(bool fullFramework, string path, string projectName)
        {
            _logger.LogInformation("Starting initial build using {0}", fullFramework ? "msbuild" : "dotnet build");
            ProcessResult result = null;
            if (fullFramework)
            {
                // Build with MSBuild.exe
                var msBuildPath = new MsBuildHelper().GetMsBuildPath();
                _logger.LogDebug("Located MSBuild.exe at: {0}", msBuildPath);

                // MSBuild inside visualstudio 2017+ contains the restore task
                if (msBuildPath.Contains("2017") || msBuildPath.Contains("2019"))
                {
                    result = _processExecutor.Start(path, msBuildPath, $"/restore");
                } else
                {
                    // try to perform a nuget restore
                    _processExecutor.Start(path, "nuget", "restore");
                    result = _processExecutor.Start(path, msBuildPath, "");
                }
            }
            else
            {
                // Build with dotnet build
                result = _processExecutor.Start(path, "dotnet", $"build {projectName}");
            }

            _logger.LogDebug("Initial build output {0}", result.Output);
            if (result.ExitCode != 0)
            {
                // Initial build failed
                throw new StrykerInputException("Initial build of targeted project failed. Please make targeted project buildable.", result.Output);
            }
            _logger.LogInformation("Initial build successful");
        }
    }
}
