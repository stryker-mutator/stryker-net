using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Stryker.Core.Initialisation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;

namespace Stryker.Core.ToolHelpers
{
    public class VsTestHelper
    {
        private readonly ProjectInfo _projectInfo;
        private readonly IFileSystem _fileSystem;

        public VsTestHelper(ProjectInfo projectInfo, IFileSystem fileSystem = null)
        {
            _projectInfo = projectInfo;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public string GetCurrentPlatformVsTestToolPath()
        {
            foreach (var path in GetVsTestToolPaths())
            {
                if (RuntimeInformation.IsOSPlatform(path.Key))
                {
                    return path.Value;
                }
            }

            throw new PlatformNotSupportedException(
                $"The current OS is not any of the following supported: " +
                $"{ OSPlatform.Windows.ToString() }, " +
                $"{ OSPlatform.Linux.ToString() } " +
                $"or " +
                $"{ OSPlatform.OSX.ToString() }");
        }

        public Dictionary<OSPlatform, string> GetVsTestToolPaths()
        {
            Dictionary<OSPlatform, string> vsTestPaths = new Dictionary<OSPlatform, string>();
            string versionString = FileVersionInfo.GetVersionInfo(typeof(IVsTestConsoleWrapper).Assembly.Location).ProductVersion;
            string portablePackageName = "microsoft.testplatform.portable";

            var nugetPackageFolders = CollectNugetPackageFolders();

            bool dllFound = false;
            bool exeFound = false;
            foreach (string nugetPackageFolder in nugetPackageFolders)
            {
                if (dllFound && exeFound)
                {
                    break;
                }

                string portablePackageFolder = _fileSystem.Directory.GetDirectories(nugetPackageFolder, portablePackageName, SearchOption.AllDirectories).First();

                string dllPath = FilePathUtils.ConvertPathSeparators(
                    Path.Combine(nugetPackageFolder, portablePackageFolder, versionString, "tools", "netcoreapp2.0", "vstest.console.dll"));
                string exePath = FilePathUtils.ConvertPathSeparators(
                    Path.Combine(nugetPackageFolder, portablePackageFolder, versionString, "tools", "net451", "vstest.console.exe"));

                if (!dllFound && _fileSystem.File.Exists(dllPath))
                {
                    vsTestPaths.Add(OSPlatform.Linux, dllPath);
                    vsTestPaths.Add(OSPlatform.OSX, dllPath);
                    dllFound = true;
                }
                if (!exeFound && _fileSystem.File.Exists(exePath))
                {
                    vsTestPaths.Add(OSPlatform.Windows, exePath);
                    exeFound = true;
                }
            }

            if (dllFound && exeFound)
            {
                return vsTestPaths;
            }
            else
            {
                throw new FileNotFoundException("VsTest executables could not be found in any of the following directories, please submit a bug report: " + string.Join(", ", nugetPackageFolders));
            }
        }

        public string GetDefaultVsTestExtensionsPath(string vstestToolPath)
        {
            string vstestMainPath = vstestToolPath.Substring(0, vstestToolPath.LastIndexOf(FilePathUtils.ConvertPathSeparators("\\")));
            string extensionPath = Path.Combine(vstestMainPath, "Extensions");
            if (_fileSystem.Directory.Exists(extensionPath))
            {
                return extensionPath;
            }
            else
            {
                throw new FileNotFoundException("VsTest test framework adapters not found at " + extensionPath);
            }
        }

        private IEnumerable<string> CollectNugetPackageFolders()
        {
            if (Environment.GetEnvironmentVariable("USERPROFILE") is var userProfile && !string.IsNullOrWhiteSpace(userProfile))
            {
                yield return Path.Combine(userProfile, ".nuget", "packages");
            }
            if (Environment.GetEnvironmentVariable("NUGET_PACKAGES") is var nugetPackagesLocation && !(string.IsNullOrWhiteSpace(nugetPackagesLocation)))
            {
                yield return Environment.GetEnvironmentVariable(@"NUGET_PACKAGES");
            }
        }
    }
}
