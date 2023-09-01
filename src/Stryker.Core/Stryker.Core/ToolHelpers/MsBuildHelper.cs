using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Testing;

namespace Stryker.Core.ToolHelpers
{
    public class MsBuildHelper
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IEnumerable<string> fallbackLocations;

        public MsBuildHelper(IFileSystem fileSystem = null, ILogger logger = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<MsBuildHelper>();
            fallbackLocations = new List<string>
            {
                @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework64\v4.0.30319\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework64\v3.5\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework64\v2.0.50727\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework\v4.0.30319\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework\v3.5\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework\v2.0.50727\MSBuild.exe",
            };
        }

        public string GetMsBuildPath(IProcessExecutor processExecutor)
        {
            // See if any MSBuild.exe can be found in visual studio installation folder
            var msBuildPath = SearchMsBuildVersion(processExecutor, "latest") ?? SearchMsBuildVersion(processExecutor, "prerelease");
            if (msBuildPath != null) return msBuildPath;
            // Else, find in default locations
            _logger.LogInformation("Unable to find msbuild using vswhere, using fallback locations");

            return fallbackLocations.FirstOrDefault(s => _fileSystem.File.Exists(s)) ?? throw new FileNotFoundException("MsBuild.exe could not be located. If you have MsBuild.exe available but still see this error please create an issue.");
        }

        private string SearchMsBuildVersion(IProcessExecutor processExecutor, string version)
        {
            foreach (var drive in Directory.GetLogicalDrives())
            {
                var visualStudioPath = Path.Combine(drive, "Program Files (x86)", "Microsoft Visual Studio");
                if (!_fileSystem.Directory.Exists(visualStudioPath)) continue;
                _logger.LogDebug("Using vswhere.exe to locate msbuild");

                var vsWherePath = Path.Combine(visualStudioPath, "Installer", "vswhere.exe");
                var vsWhereCommand =
                    $"-{version} -requires Microsoft.Component.MSBuild -products * -find MSBuild\\**\\Bin\\MSBuild.exe";
                var vsWhereResult = processExecutor.Start(visualStudioPath, vsWherePath, vsWhereCommand);

                if (vsWhereResult.ExitCode == ExitCodes.Success)
                {
                    var msBuildPath = vsWhereResult.Output.Trim();
                    if (_fileSystem.File.Exists(msBuildPath))
                    {
                        _logger.LogDebug($"Msbuild executable path found at {msBuildPath}");

                        return msBuildPath;
                    }
                }
            }

            return null;
        }
    }
}
