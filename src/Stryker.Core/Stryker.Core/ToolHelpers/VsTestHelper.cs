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
        private readonly object _firstTimeVsTestDeployLock = new object();
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<OSPlatform, string> _vstestPaths = new Dictionary<OSPlatform, string>();

        private static VsTestHelper _vsTestHelper;

        private VsTestHelper(StrykerOptions options, IFileSystem fileSystem = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public static VsTestHelper GetVsTestHelper(StrykerOptions options, IFileSystem fileSystem = null)
        {
            _vsTestHelper = _vsTestHelper ?? new VsTestHelper(options, fileSystem);
            return _vsTestHelper;
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

        private Dictionary<OSPlatform, string> GetVsTestToolPaths()
        {
            if (!(_vstestPaths is null))
            {
                return _vstestPaths;
            }

            lock (_firstTimeVsTestDeployLock)
            {
                if (_vstestPaths is null)
                {
                    var nugetPackageFolders = CollectNugetPackageFolders();

                    if (SearchNugetPackageFolders(nugetPackageFolders) is var nugetAssemblies && !(nugetAssemblies is null))
                    {
                        Merge(_vstestPaths, nugetAssemblies);
                    }
                    if (DeployEmbeddedVsTestBinaries() is var deployedPaths && !(deployedPaths is null))
                    {
                        Merge(_vstestPaths, deployedPaths);
                    }
                    if (_vstestPaths.Count == 0)
                    {
                        throw new ApplicationException("Could not find or deploy vstest. Exiting.");
                    }
                }
            }

            return _vstestPaths;
        }

        private void Merge(Dictionary<OSPlatform, string> to, Dictionary<OSPlatform, string> from)
        {
            foreach (var val in from)
            {
                to[val.Key] = val.Value;
            }
        }

        private Dictionary<OSPlatform, string> SearchNugetPackageFolders(IEnumerable<string> nugetPackageFolders)
        {
            Dictionary<OSPlatform, string> vsTestPaths = new Dictionary<OSPlatform, string>();
            string versionString = FileVersionInfo.GetVersionInfo(typeof(IVsTestConsoleWrapper).Assembly.Location).ProductVersion;
            string portablePackageName = "microsoft.testplatform.portable";
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
                    vsTestPaths[OSPlatform.Linux] = dllPath;
                    vsTestPaths[OSPlatform.OSX] = dllPath;
                    dllFound = true;
                }
                if (!exeFound && _fileSystem.File.Exists(exePath))
                {
                    vsTestPaths[OSPlatform.Windows] = exePath;
                    exeFound = true;
                }
            }

            return vsTestPaths;
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
                    var path = Path.Combine(_options.OutputPath, ".vstest", $"vstest.console{extension}");
                    _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(path));

                    using (var file = _fileSystem.FileStream.Create(path, FileMode.Create))
                    {
                        stream.CopyTo(file);
                    }

                    if (extension == "exe")
                    {
                        paths[OSPlatform.Windows] = path;
                    }
                    else
                    {
                        paths[OSPlatform.Linux] = path;
                        paths[OSPlatform.OSX] = path;
                    }
                }
            }

            return paths;
        }


    }
}
