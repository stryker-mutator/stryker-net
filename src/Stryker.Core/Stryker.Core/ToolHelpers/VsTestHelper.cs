using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Stryker.Core.Logging;

namespace Stryker.Core.ToolHelpers
{
    public interface IVsTestHelper
    {
        string GetCurrentPlatformVsTestToolPath();
        void Cleanup(int tries = 5);
    }

    public class VsTestHelper : IVsTestHelper
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<OSPlatform, string> _vstestPaths = new Dictionary<OSPlatform, string>();
        private string _platformVsTestToolPath;
        private object _lck = new object();

        public VsTestHelper(IFileSystem fileSystem = null, ILogger logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestHelper>();
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public string GetCurrentPlatformVsTestToolPath()
        {
            lock (_lck)
            {
                if (string.IsNullOrEmpty(_platformVsTestToolPath))
                {
                    var paths = GetVsTestToolPaths();
                    foreach (var path in paths)
                    {
                        if (RuntimeInformation.IsOSPlatform(path.Key))
                        {
                            _logger.LogDebug("Using vstest.console: {0} for OS {1}", path.Value, path.Key);
                            _platformVsTestToolPath = path.Value;
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(_platformVsTestToolPath))
                    {
                        throw new PlatformNotSupportedException(
                            $"The current OS is not any of the following currently supported: {string.Join(", ", paths.Keys)}");
                    }
                }
            }

            return _platformVsTestToolPath;
        }

        private Dictionary<OSPlatform, string> GetVsTestToolPaths()
        {
            // If any of the found paths is for the current OS, just return the paths as we have what we need
            if (_vstestPaths.Any(p => RuntimeInformation.IsOSPlatform(p.Key)))
            {
                return _vstestPaths;
            }

            var nugetPackageFolders = CollectNugetPackageFolders();

            if (SearchNugetPackageFolders(nugetPackageFolders) is var nugetAssemblies
                && nugetAssemblies.Any(p => RuntimeInformation.IsOSPlatform(p.Key)))
            {
                Merge(_vstestPaths, nugetAssemblies);
                _logger.LogDebug("Using vstest from nuget package folders");
            }
            else if (DeployEmbeddedVsTestBinaries() is var deployPath)
            {
                Merge(_vstestPaths, SearchNugetPackageFolders(new List<string> { deployPath }, versionDependent: false));
                _logger.LogDebug("Using vstest from deployed vstest package");
            }
            else
            {
                throw new ApplicationException("Could not find or deploy vstest. Exiting.");
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
                if (dllFound && exeFound)
                {
                    break;
                }

                string searchFolder;
                if (versionDependent)
                {
                    var portablePackageFolder = Path.Combine(nugetPackageFolder, portablePackageName, versionString);

                    if (!_fileSystem.Directory.Exists(portablePackageFolder))
                    {
                        return vsTestPaths;
                    }
                    searchFolder = portablePackageFolder;
                }
                else
                {
                    searchFolder = nugetPackageFolder;
                }

                var dllPath = FilePathUtils.NormalizePathSeparators(
                    _fileSystem.Directory.GetFiles(
                        searchFolder,
                        "vstest.console.dll",
                        SearchOption.AllDirectories).FirstOrDefault());
                var exePath = FilePathUtils.NormalizePathSeparators(
                    _fileSystem.Directory.GetFiles(
                        searchFolder,
                        "vstest.console.exe",
                        SearchOption.AllDirectories).FirstOrDefault());

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
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var path = Path.Combine(userProfilePath, ".nuget", "packages");
            if (_fileSystem.Directory.Exists(path))
            {
                yield return path;
            }

            static bool TryGetNonEmptyEnvironmentVariable(string variable, out string value)
            {
                value = Environment.GetEnvironmentVariable(variable);
                return !string.IsNullOrWhiteSpace(value);
            }

            if (TryGetNonEmptyEnvironmentVariable("NUGET_PACKAGES", out var nugetPackagesLocation)
                && _fileSystem.Directory.Exists(nugetPackagesLocation))
            {
                yield return nugetPackagesLocation;
            }
        }

        private string DeployEmbeddedVsTestBinaries()
        {
            var vstestZip = typeof(VsTestHelper).Assembly
                .GetManifestResourceNames()
                .Single(r => r.Contains("Microsoft.TestPlatform.Portable", StringComparison.InvariantCultureIgnoreCase));

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), ".vstest");

            using (var stream = typeof(VsTestHelper).Assembly
            .GetManifestResourceStream(vstestZip))
            {
                var zipPath = Path.Combine(tempDir, "vstest.zip");
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
