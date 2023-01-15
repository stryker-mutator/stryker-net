using System;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.Initialisation
{
    public interface IInitialBuildProcess
    {
        void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string msbuildPath = null,
            bool isUnity = false);
    }

    public class InitialBuildProcess : IInitialBuildProcess
    {
        private readonly IFileSystem _fileSystem;
        private readonly IProcessExecutor _processExecutor;
        private readonly ILogger _logger;

        public InitialBuildProcess(IFileSystem fileSystem, IProcessExecutor processExecutor = null)
        {
            _fileSystem = fileSystem;
            _processExecutor = processExecutor ?? new ProcessExecutor();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialBuildProcess>();
        }

        public void InitialBuild(bool fullFramework, string projectPath, string solutionPath, string msbuildPath = null,
            bool isUnity = false)
        {
            _logger.LogDebug("Started initial build using {0}", fullFramework ? "msbuild.exe" : "dotnet build");

            ProcessResult result;
            if (isUnity)
            {
                //todo run unity to generate csproj https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/extension-retrieve-test-list.html

                var info = new FileInfo(projectPath);
                foreach (var fileInfo in info.Directory?.GetFiles("*.csproj")!)
                {
                    UpdateOutputPathForUnityProjects(fileInfo.FullName);
                }

                CopyUnitySDKInTargetUnityProject(projectPath);
            }
            else if (fullFramework)
            {
                if (string.IsNullOrEmpty(solutionPath))
                {
                    throw new InputException(
                        "Stryker could not build your project as no solution file was presented. Please pass the solution path to stryker.");
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

                CheckBuildResult(result, "dotnet build", $"\"{Path.GetFileName(projectPath)}\"");
            }
        }

        private void CopyUnitySDKInTargetUnityProject(string pathProject)
        {
            var unityProjectPath = Directory.GetParent(pathProject).FullName;
            var allFilesOfSdk = typeof(VsTestHelper).Assembly
                .GetManifestResourceNames().Where(name => name.Contains("Stryker.UnitySDK")) ?? throw new ArgumentNullException("typeof(VsTestHelper).Assembly\n                .GetManifestResourceNames().Where(name => name.Contains(\"Stryker.UnitySDK\"))");

            var pathToPackageOfSdk = Path.Combine(unityProjectPath, "Packages", "Stryker.UnitySDK");
            Directory.CreateDirectory(pathToPackageOfSdk);

            File.WriteAllText(Path.Combine(pathToPackageOfSdk, ".gitignore"), "*");
            foreach (var nameEmbeddedResource in allFilesOfSdk)
            {
                using var file = _fileSystem.FileStream.New(
                    Path.Combine(pathToPackageOfSdk, GetFinalNameOfResource(nameEmbeddedResource)),
                    FileMode.Create);

                typeof(VsTestHelper).Assembly.GetManifestResourceStream(nameEmbeddedResource).CopyTo(file);
            }

            string GetFinalNameOfResource(string name)
            {
                return name.Split("Stryker.UnitySDK.").Last();
            }
        }

        private static void UpdateOutputPathForUnityProjects(string projectPath)
        {
            var pattern = new Regex(@"<OutputPath>.*<\/OutputPath>", RegexOptions.Compiled);
            var updatedCsProjContent = pattern.Replace(File.ReadAllText(projectPath),
                _ => @"<OutputPath>Library\ScriptAssemblies\</OutputPath>");
            File.WriteAllText(projectPath, updatedCsProjContent);
        }

        private void CheckBuildResult(ProcessResult result, string buildCommand, string buildPath)
        {
            _logger.LogTrace("Initial build output {0}", result.Output);
            if (result.ExitCode != ExitCodes.Success)
            {
                // Initial build failed
                throw new InputException(result.Output,
                    $"Initial build of targeted project failed. Please make sure the targeted project is buildable. You can reproduce this error yourself using: \"{buildCommand} {buildPath}\"");
            }

            _logger.LogDebug("Initial build successful");
        }
    }
}
