using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Stryker.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;

namespace Stryker.Core.ToolHelpers
{
    public class VsTestHelper
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<OSPlatform, string> _vstestPaths = new Dictionary<OSPlatform, string>();
        private string _platformVsTestToolPath;

        public VsTestHelper(IFileSystem fileSystem = null, ILogger logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestHelper>();
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public string GetCurrentPlatformVsTestToolPath()
        {
            if (string.IsNullOrEmpty(_platformVsTestToolPath))
            {
                foreach (var path in GetVsTestToolPaths())
                {
                    if (RuntimeInformation.IsOSPlatform(path.Key))
                    {
                        _logger.LogDebug("Using vstest.console: {0}", path.Value);
                        _platformVsTestToolPath = path.Value;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(_platformVsTestToolPath))
            {
                throw new PlatformNotSupportedException(
                $"The current OS is not any of the following supported: " +
                $"{ OSPlatform.Windows.ToString() }, " +
                $"{ OSPlatform.Linux.ToString() } " +
                $"or " +
                $"{ OSPlatform.OSX.ToString() }");
            }

            return _platformVsTestToolPath;
        }

        public string GetDefaultVsTestExtensionsPath(string vstestToolPath)
        {
            var extensionPath = Path.Combine(Path.GetDirectoryName(vstestToolPath), "Extensions");
            if (_fileSystem.Directory.Exists(extensionPath))
            {
                return extensionPath;
            }
            throw new ApplicationException($"VsTest extensions not found in: {extensionPath}");
        }

        private Dictionary<OSPlatform, string> GetVsTestToolPaths()
        {
            // If any of the found paths is for the current OS, just return the paths as we have what we need
            if (_vstestPaths.Any(p => RuntimeInformation.IsOSPlatform(p.Key)))
            {
                return _vstestPaths;
            }

            if (_vstestPaths.Count == 0)
            {
                var nugetPackageFolders = CollectNugetPackageFolders();

                if (SearchNugetPackageFolders(nugetPackageFolders) is var nugetAssemblies && nugetAssemblies.Count != 0)
                {
                    Merge(_vstestPaths, nugetAssemblies);
                }
                else if (DeployEmbeddedVsTestBinaries() is var deployPath)
                {
                    Merge(_vstestPaths, SearchNugetPackageFolders(new List<string> { deployPath }, versionDependent: false));
                }
                else
                {
                    throw new ApplicationException("Could not find or deploy vstest. Exiting.");
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

        private Dictionary<OSPlatform, string> SearchNugetPackageFolders(IEnumerable<string> nugetPackageFolders, bool versionDependent = true)
        {
            var vsTestPaths = new Dictionary<OSPlatform, string>();

            var versionString = FileVersionInfo.GetVersionInfo(typeof(IVsTestConsoleWrapper).Assembly.Location).ProductVersion;
            const string portablePackageName = "microsoft.testplatform.portable";
            bool dllFound = false, exeFound = false;

            foreach (string nugetPackageFolder in nugetPackageFolders)
            {
                string dllPath = null, exePath = null;
                if (dllFound && exeFound)
                {
                    break;
                }

                if (versionDependent)
                {
                    var portablePackageFolder = _fileSystem.Directory.GetDirectories(nugetPackageFolder, portablePackageName, SearchOption.AllDirectories).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(portablePackageFolder))
                    {
                        return vsTestPaths;
                    }

                    dllPath = FilePathUtils.ConvertPathSeparators(
                        Path.Combine(
                            nugetPackageFolder, portablePackageFolder, versionString,
                            "tools", "netcoreapp2.0", "vstest.console.dll"));
                    exePath = FilePathUtils.ConvertPathSeparators(
                        Path.Combine(
                            nugetPackageFolder, portablePackageFolder, versionString,
                            "tools", "net451", "vstest.console.exe"));
                }
                else
                {
                    dllPath = FilePathUtils.ConvertPathSeparators(
                        _fileSystem.Directory.GetFiles(
                            nugetPackageFolder,
                            "vstest.console.dll",
                            SearchOption.AllDirectories).First());
                    exePath = FilePathUtils.ConvertPathSeparators(
                        _fileSystem.Directory.GetFiles(
                            nugetPackageFolder,
                            "vstest.console.exe",
                            SearchOption.AllDirectories).First());
                }

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

        private static IEnumerable<string> CollectNugetPackageFolders()
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

        private string DeployEmbeddedVsTestBinaries()
        {
            var vstestZip = typeof(VsTestHelper).Assembly
                .GetManifestResourceNames()
                .Single(r => r.Contains("Microsoft.TestPlatform.Portable"));

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), ".vstest");

            using (var stream = typeof(VsTestHelper).Assembly
            .GetManifestResourceStream(vstestZip))
            {
                var zipPath = Path.Combine(tempDir, $"vstest.zip");
                _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(zipPath));

                using (var file = _fileSystem.FileStream.Create(zipPath, FileMode.Create))
                {
                    stream.CopyTo(file);
                }

                _logger.LogDebug("VsTest zip was copied to: {0}", zipPath);

                ZipFile.ExtractToDirectory(zipPath, tempDir);
                _fileSystem.File.Delete(zipPath);

                _logger.LogDebug("VsTest zip was unzipped to: {0}", tempDir);
            }

            return tempDir;
        }

        public void Cleanup(int tries = 5)
        {
            var nugetPackageFolders = CollectNugetPackageFolders();

            try
            {
                foreach (var vstestConsole in _vstestPaths)
                {
                    var path = vstestConsole.Value;
                    // If vstest path is not in nuget package folder, clean it up
                    if (!nugetPackageFolders.Any(nf => path.Contains(nf)))
                    {
                        if (_fileSystem.Directory.Exists(Path.GetDirectoryName(path)))
                        {
                            foreach (var entry in _fileSystem.Directory
                                .EnumerateFiles(Path.GetDirectoryName(path), "*", SearchOption.AllDirectories))
                            {
                                _fileSystem.File.Delete(entry);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                _logger.LogDebug($"Tried cleaning up used vstest resources but we weren't ready to clean. " +
                    $"{(tries != 0 ? $"Trying {tries} more times." : "Out of tries, we're giving up sorry.")}");
                if (tries > 0)
                {
                    Cleanup(tries - 1);
                }
            }
        }
    }
}
