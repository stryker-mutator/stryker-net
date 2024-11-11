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
using Stryker.Abstractions.Logging;
using Stryker.Utilities;

namespace Stryker.Core.Helpers;

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
            _logger.LogDebug("Using vstest.console: {OsPlatform} for OS {TestToolPath}",
                osPlatform, _platformVsTestToolPath);
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
            Merge(_vsTestPaths,
                SearchNugetPackageFolders(new List<string> { deployPath }, versionDependent: false));
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

    private Dictionary<OSPlatform, string> SearchNugetPackageFolders(IEnumerable<string> nugetPackageFolders,
        bool versionDependent = true)
    {
        var vsTestPaths = new Dictionary<OSPlatform, string>();

        var versionString = FileVersionInfo.GetVersionInfo(typeof(IVsTestConsoleWrapper).Assembly.Location)
            .ProductVersion;
        const string PortablePackageName = "microsoft.testplatform.portable";
        bool dllFound = false, exeFound = false;

        foreach (var nugetPackageFolder in nugetPackageFolders)
        {
            var searchFolder = versionDependent
                ? Path.Combine(nugetPackageFolder, PortablePackageName, versionString)
                : nugetPackageFolder;
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

    internal string DeployEmbeddedVsTestBinaries()
    {
        var assembly = typeof(VsTestHelper).Assembly;
        var vsTestZips = assembly.GetManifestResourceNames().Where(r => r == "Microsoft.TestPlatform.Portable.nupkg").ToList();

        var vsTestZip = vsTestZips.Count switch
        {
            0 => throw new InvalidOperationException($"The Microsoft.TestPlatform.Portable.nupkg embedded resource was not found in {assembly.GetName().Name}. " +
                                                     "Please report this issue at https://github.com/stryker-mutator/stryker-net/issues"),
            1 => vsTestZips[0],
            _ => throw new InvalidOperationException($"Multiple Microsoft.TestPlatform.Portable.nupkg embedded resources were found in {assembly.GetName().Name}. " +
                                                     "Please report this issue at https://github.com/stryker-mutator/stryker-net/issues"),
        };

        using var stream = assembly.GetManifestResourceStream(vsTestZip)
                           ?? throw new InvalidOperationException($"Failed to get the resource stream of {vsTestZip} in {assembly.GetName().Name}. " +
                                                                  "Please report this issue at https://github.com/stryker-mutator/stryker-net/issues");

        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), ".vstest");
        var zipPath = Path.Combine(tempDir, "vstest.zip");
        _fileSystem.Directory.CreateDirectory(tempDir);

        using (var file = _fileSystem.FileStream.New(zipPath, FileMode.Create))
        {
            stream.CopyTo(file);
        }

        _logger.LogDebug("VsTest zip was copied to: {ZipPath}", zipPath);

        ZipFile.ExtractToDirectory(zipPath, tempDir);
        _fileSystem.File.Delete(zipPath);

        _logger.LogDebug("VsTest zip was unzipped to: {TempDir}", tempDir);

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
                if (nugetPackageFolders.Any(nf => path.Contains(nf)) ||
                    !_fileSystem.Directory.Exists(Path.GetDirectoryName(path)))
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
        catch (Exception ex)
        {
            if (tries > 0)
            {
                _logger.LogDebug(ex,
                    "Tried cleaning up used vstest resources but we weren't ready to clean. Trying {tries} more times.",
                    tries);
                Cleanup(tries - 1);
            }
            else
            {
                _logger.LogWarning(ex,
                    "Tried cleaning up used vstest resources but we weren't ready to clean. Out of tries, we're giving up sorry.");
            }
        }
    }
}
