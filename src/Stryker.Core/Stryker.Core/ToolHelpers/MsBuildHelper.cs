﻿using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

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
                @"C:\Windows\Microsoft.Net\Framework64\v4.0.30319\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework64\v3.5\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework64\v2.0.50727\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework\v4.0.30319\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework\v3.5\MSBuild.exe",
                @"C:\Windows\Microsoft.Net\Framework\v2.0.50727\MSBuild.exe",
            };
        }

        public string GetMsBuildPath(IProcessExecutor _processExecutor)
        {
            // See if any MSBuild.exe can be found in visual studio installation folder
            var visualStudioPath = Path.Combine("C:", "Program Files (x86)", "Microsoft Visual Studio");
            if (_fileSystem.Directory.Exists(visualStudioPath))
            {
                _logger.LogDebug("Using vswhere.exe to locate msbuild");

                var vsWherePath = Path.Combine(visualStudioPath, "Installer", "vswhere.exe");
                var vsWhereCommand = "-latest -requires Microsoft.Component.MSBuild -find MSBuild\\**\\Bin\\MSBuild.exe";
                var vsWhereResult = _processExecutor.Start(visualStudioPath, vsWherePath, vsWhereCommand);

                if (vsWhereResult.ExitCode == 0)
                {
                    var msBuildPath = vsWhereResult.Output.Trim();
                    if (_fileSystem.File.Exists(msBuildPath))
                    {
                        _logger.LogDebug($"Msbuild executable path found at {msBuildPath}");

                        return msBuildPath;
                    }
                }
            }

            // Else, find in default locations
            _logger.LogDebug("Unable to find msbuild using vswhere, using fallback locations");

            foreach (string possiblePath in fallbackLocations)
            {
                if (_fileSystem.File.Exists(possiblePath))
                {
                    return possiblePath;
                }
            }
            throw new FileNotFoundException("MsBuild.exe could not be located. If you have MsBuild.exe available but still see this error please create an issue.");
        }
    }
}
