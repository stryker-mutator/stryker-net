using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Stryker.Core.Options;
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
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private Dictionary<OSPlatform, string> _vstestPaths;

        public VsTestHelper(StrykerOptions options, IFileSystem fileSystem = null)
        {
            _options = options;
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
            if (!(_vstestPaths is null))
            {
                return _vstestPaths;
            }

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
                _vstestPaths = vsTestPaths;
            }
            else
            {
                _vstestPaths = DeployEmbeddedVsTestBinaries();
            }

            return _vstestPaths;
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

        private Dictionary<OSPlatform, string> DeployEmbeddedVsTestBinaries()
        {
            var paths = new Dictionary<OSPlatform, string>();

            var vsTestResources = typeof(VsTestHelper).Assembly
                .GetManifestResourceNames()
                .Where(r => r.Contains("vstest.console"));

            foreach (var vstest in vsTestResources)
            {
                using (var stream = typeof(VsTestHelper).Assembly
                .GetManifestResourceStream(vstest))
                {
                    var extension = Path.GetExtension(vstest);
                    var path = Path.Combine(_options.OutputPath, ".vstest", $"vstest.console.{extension}");

                    using (var file = _fileSystem.FileStream.Create(path, FileMode.Truncate))
                    {
                        stream.CopyTo(file);
                    }

                    if (extension == "exe")
                    {
                        paths.Add(OSPlatform.Windows, path);
                    }
                    else
                    {
                        paths.Add(OSPlatform.Linux, path);
                        paths.Add(OSPlatform.OSX, path);
                    }
                }
            }

            return paths;
        }
    }
}
