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
using Stryker.Utilities;
using Stryker.Utilities.Logging;

namespace Stryker.TestRunner.VsTest.Helpers;

public interface IVsTestHelper
{
    string GetCurrentPlatformVsTestToolPath();
    void Cleanup(int tries = 5);
}

/// <summary>
/// Locates VsTest folder. Installs one if none is found.
/// </summary>
/// This class is not unit tested currently, so proceed with caution
// [ExcludeFromCodeCoverage(Justification = "Deeply dependent on current platform, need a lot of work for mocking.")]
public abstract class VsTestHelper : IVsTestHelper
{
    private readonly ILogger _logger;
    protected readonly IFileSystem _fileSystem;
    private string _platformVsTestToolPath;
    private readonly object _lck = new();
    private List<string> dirsToClean = new();

    protected VsTestHelper(IFileSystem fileSystem, ILogger logger)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public static IVsTestHelper CreateInstance(IFileSystem? fileSystem = null, ILogger? logger = null, Func<OSPlatform, bool>? isOsPlatform = null)
    {
        logger ??= ApplicationLogging.LoggerFactory.CreateLogger<VsTestHelper>();
        fileSystem ??= new FileSystem();
        isOsPlatform ??= RuntimeInformation.IsOSPlatform;

        if (isOsPlatform(OSPlatform.Linux) || isOsPlatform(OSPlatform.OSX))
        {
            return new UnixTestHelper(fileSystem, logger);
        }
        if (isOsPlatform(OSPlatform.Windows))
        {
            return new WindowsTestHelper(fileSystem, logger);
        }

        return new NotSupportedTestHelper(OSPlatform.Windows, OSPlatform.Linux, OSPlatform.OSX);
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

            var platformVsTestToolPath = GetVsTestToolPaths();
            _platformVsTestToolPath = platformVsTestToolPath ?? throw new PlatformNotSupportedException(
                "Could not find any VS test tool paths for this platform.");

            var osPlatform = "Bla";
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
    private string? GetVsTestToolPaths()
    {
        var searchNugetPackageFolders = SearchNugetPackageFolders(CollectNugetPackageFolders());
        if (searchNugetPackageFolders is not null)
        {

            _logger.LogDebug("Using vstest from nuget package folders");
            return searchNugetPackageFolders;
        }
        else if (DeployEmbeddedVsTestBinaries() is var deployPath)
        {
            dirsToClean.Add(deployPath);
            var packageFolders = SearchNugetPackageFolders(new List<string> { deployPath }, versionDependent: false);
            if (packageFolders is not null)
            {
                _logger.LogDebug("Using vstest from deployed vstest package");
                return packageFolders;
            }
        }

        return null;
    }

    protected abstract string? SearchNugetPackageFolders(IEnumerable<string> nugetPackageFolders,
        bool versionDependent = true);

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

    public string DeployEmbeddedVsTestBinaries()
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
        try
        {
            foreach (var dir in dirsToClean)
            {
                foreach (var entry in _fileSystem.Directory
                             .EnumerateFiles(dir, "*", SearchOption.AllDirectories))
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

public class NotSupportedTestHelper : IVsTestHelper
{
    private IEnumerable<OSPlatform> supported;

    public NotSupportedTestHelper(params OSPlatform[] supported) => this.supported = supported;

    public string GetCurrentPlatformVsTestToolPath()
    {
        throw new PlatformNotSupportedException(
            $"The current OS is not any of the following currently supported: {string.Join(", ", supported)}");
    }

    public void Cleanup(int tries = 5) => throw new NotImplementedException();
}

public class UnixTestHelper : VsTestHelper
{
    protected internal UnixTestHelper(IFileSystem fileSystem, ILogger logger) : base(fileSystem, logger)
    {
    }

    protected override string? SearchNugetPackageFolders(IEnumerable<string> nugetPackageFolders,
        bool versionDependent = true)
    {
        var versionString = FileVersionInfo.GetVersionInfo(typeof(IVsTestConsoleWrapper).Assembly.Location)
            .ProductVersion;
        const string PortablePackageName = "microsoft.testplatform.portable";

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
            if (_fileSystem.File.Exists(dllPath))
            {
                return dllPath;
            }
        }

        return null;
    }
}

public class WindowsTestHelper : VsTestHelper
{
    protected internal WindowsTestHelper(IFileSystem fileSystem, ILogger logger) : base(fileSystem, logger)
    {
    }

    protected override string? SearchNugetPackageFolders(IEnumerable<string> nugetPackageFolders,
        bool versionDependent = true)
    {
        var versionString = FileVersionInfo.GetVersionInfo(typeof(IVsTestConsoleWrapper).Assembly.Location)
            .ProductVersion;
        const string PortablePackageName = "microsoft.testplatform.portable";

        foreach (var nugetPackageFolder in nugetPackageFolders)
        {
            var searchFolder = versionDependent
                ? Path.Combine(nugetPackageFolder, PortablePackageName, versionString)
                : nugetPackageFolder;
            if (!_fileSystem.Directory.Exists(searchFolder))
            {
                continue;
            }

            var exePath = FilePathUtils.NormalizePathSeparators(
                _fileSystem.Directory.GetFiles(
                    searchFolder,
                    "vstest.console.exe",
                    SearchOption.AllDirectories).FirstOrDefault());


            if (_fileSystem.File.Exists(exePath))
            {
                return exePath;
            }
        }

        return null;
    }
}
