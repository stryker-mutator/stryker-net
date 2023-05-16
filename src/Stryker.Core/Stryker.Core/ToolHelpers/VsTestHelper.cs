using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Locates VsTest folder. Installs one if none is found. 
    /// </summary>
    /// This class is not unit tested currently, so proceed with caution
    [ExcludeFromCodeCoverage(Justification = "Deeply dependent on current platform, need a lot of work for mocking.")]
    public class VsTestHelper : IVsTestHelper
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<OSPlatform, string> _vsTestPaths = new();
        private string _platformVsTestToolPath;
        private readonly object _lck = new();

        public VsTestHelper(IFileSystem fileSystem = null, ILogger logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestHelper>();
            _fileSystem = fileSystem ?? new FileSystem();
        }

        /// <summary>
        /// Finds VsTest path version corresponding to current platform (ie OS)
        /// </summary>
        /// <returns>VsTest full path.</returns>
        /// <exception cref="PlatformNotSupportedException">When it fails to find a VsTest install for the current platform</exception>
        public string GetCurrentPlatformVsTestToolPath()
        {
            lock (_lck)
            {
                if (!string.IsNullOrEmpty(_platformVsTestToolPath))
                {
                    return _platformVsTestToolPath;
                }
                var paths = GetVsTestToolPaths();

                if (!paths.Keys.Any(RuntimeInformation.IsOSPlatform))
                {
                    throw new PlatformNotSupportedException(
                        $"The current OS is not any of the following currently supported: {string.Join(", ", paths.Keys)}");
                }

                var osPlatform = paths.Keys.First(RuntimeInformation.IsOSPlatform);
                _platformVsTestToolPath = paths[osPlatform];
                _logger.LogDebug("Using vstest.console: {0} for OS {1}", osPlatform, _platformVsTestToolPath);
            }

            return _platformVsTestToolPath;
        }

        /// <summary>
        /// Gets VsTest installations
        /// </summary>
        /// <returns>a dictionary with the folder for each detected platform</returns>
        /// <exception cref="ApplicationException">If it fails to find and deploy VsTest</exception>
        private Dictionary<OSPlatform, string> GetVsTestToolPaths()
        {
            // If any of the found paths is for the current OS, just return the paths as we have what we need
            if (_vsTestPaths.Any(p => RuntimeInformation.IsOSPlatform(p.Key)))
            {
                return _vsTestPaths;
            }

            var nugetPackageFolders = CollectNugetPackageFolders();

            if (SearchNugetPackageFolders(nugetPackageFolders) is var nugetAssemblies
                && nugetAssemblies.Any(p => RuntimeInformation.IsOSPlatform(p.Key)))
            {
                Merge(_vsTestPaths, nugetAssemblies);
                _logger.LogDebug("Using vstest from nuget package folders");
            }
            else if (DeployEmbeddedVsTestBinaries() is var deployPath)
            {
                Merge(_vsTestPaths, SearchNugetPackageFolders(new List<string> { deployPath }, versionDependent: false));
                _logger.LogDebug("Using vstest from deployed vstest package");
            }

            return _vsTestPaths;
        }

        private static void Merge(IDictionary<OSPlatform, string> to, Dictionary<OSPlatform, string> from)
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
            const string PortablePackageName = "microsoft.testplatform.portable";
            bool dllFound = false, exeFound = false;

            foreach (var nugetPackageFolder in nugetPackageFolders)
            {
                var searchFolder = versionDependent ? Path.Combine(nugetPackageFolder, PortablePackageName, versionString) : nugetPackageFolder;
                if (!_fileSystem.Directory.Exists(searchFolder))
                {
                    continue;
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
                if (dllFound && exeFound)
                {
                    break;
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
            var vsTestZip = typeof(VsTestHelper).Assembly
                .GetManifestResourceNames()
                .Single(r => r.Contains("Microsoft.TestPlatform.Portable", StringComparison.InvariantCultureIgnoreCase));

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), ".vstest");

            using var stream = typeof(VsTestHelper).Assembly
                .GetManifestResourceStream(vsTestZip);
            var zipPath = Path.Combine(tempDir, "vstest.zip");
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(zipPath));

            using (var file = _fileSystem.FileStream.New(zipPath, FileMode.Create))
            {
                stream.CopyTo(file);
            }

            _logger.LogDebug("VsTest zip was copied to: {0}", zipPath);

            ZipFile.ExtractToDirectory(zipPath, tempDir);
            _fileSystem.File.Delete(zipPath);

            _logger.LogDebug("VsTest zip was unzipped to: {0}", tempDir);

            return tempDir;
        }

        /// <summary>
        /// Removes any VsTest instance that were deployed by Stryker.
        /// </summary>
        /// <param name="tries">Remaining attempts.</param>
        public void Cleanup(int tries = 5)
        {
            var nugetPackageFolders = CollectNugetPackageFolders();

            try
            {
                foreach (var vsTestConsole in _vsTestPaths.Values)
                {
                    var path = vsTestConsole;
                    // If vstest path is not in nuget package folder, clean it up
                    if (nugetPackageFolders.Any(nf => path.Contains(nf)) || !_fileSystem.Directory.Exists(Path.GetDirectoryName(path)))
                    {
                        continue;
                    }

                    foreach (var entry in _fileSystem.Directory
                                 .EnumerateFiles(Path.GetDirectoryName(path), "*", SearchOption.AllDirectories))
                    {
                        _fileSystem.File.Delete(entry);
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
