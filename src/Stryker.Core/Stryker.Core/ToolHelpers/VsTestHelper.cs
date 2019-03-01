using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Stryker.Core.ToolHelpers
{
    public class VsTestHelper
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;

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
            Dictionary<OSPlatform, string> vsTestPaths = new Dictionary<OSPlatform, string>();
            string versionString = "16.0.0";
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
            yield return Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), ".nuget", "packages");
            if (Environment.GetEnvironmentVariable("NUGET_PACKAGES") is var nugetPackagesLocation && !(nugetPackagesLocation is null))
            {
                yield return Environment.GetEnvironmentVariable(@"NUGET_PACKAGES");
            }
            foreach (string nugetPackageFolder in ParseNugetPackageFolders())
            {
                yield return nugetPackageFolder;
            }
        }

        private IEnumerable<string> ParseNugetPackageFolders()
        {
            string nugetPropsLocation = Path.Combine(_options.BasePath, "obj", $"{_options.ProjectUnderTestNameFilter}.nuget.g.props");

            if (_fileSystem.File.Exists(nugetPropsLocation))
            {
                XElement document = XElement.Load(nugetPropsLocation);
                string nugetPackageFolderElementValue = document.Descendants("NuGetPackageFolders").Select(e => e.Value).First();
                string[] nugetPackageFolders = nugetPackageFolderElementValue.Split(";");

                foreach (string nugetPackageFolder in nugetPackageFolders)
                {
                    yield return nugetPackageFolder;
                }
            }
            else
            {
                yield break;
            }
        }
    }
}
