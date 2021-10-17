using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.ToolHelpers
{
    public interface INugetHelper
    {
        IEnumerable<string> CollectNugetPackageFolders();
        bool TryGetPackagePath(string packageName, out string path);
        bool CopyPackageTo(string packageName, string destinationPath);
    }

    public class NugetHelper : INugetHelper
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;

        public NugetHelper(IFileSystem fileSystem = null, ILogger logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestHelper>();
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public bool CopyPackageTo(string packageName, string destinationPath)
        {
            if (!TryGetPackagePath(packageName, out var nugetFile))
                return false;

            var assemblyFileName = Path.GetFileName(nugetFile);
            var destFilePath = Path.Combine(destinationPath, assemblyFileName);

            if (!File.Exists(destFilePath))
                File.Copy(nugetFile, destFilePath);

            return true;
        }

        public bool TryGetPackagePath(string packageName, out string path)
        {
            path = string.Empty;
            var dllToFind = packageName;

            if (!dllToFind.EndsWith(".dll"))
                dllToFind = $"{dllToFind}.dll";

            var nugetPackageFolders = CollectNugetPackageFolders();

            foreach (var folder in nugetPackageFolders)
            {
                var files = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
                var nugetPath = files.FirstOrDefault(f => f.EndsWith(dllToFind));

                if (!string.IsNullOrWhiteSpace(nugetPath))
                {
                    path = nugetPath;
                    return true;
                }
            }

            _logger.LogWarning($"Package not found with name: {packageName}");
            return false;
        }

        public IEnumerable<string> CollectNugetPackageFolders()
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
    }
}
