using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Stryker.Core.ToolHelpers
{
    public class MsBuildHelper
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<string> fallbackLocations;

        public MsBuildHelper(IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
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

        public string GetMsBuildPath()
        {
            // See if any MSBuild.exe can be found in visual studio installation folder
            if (_fileSystem.Directory.Exists(@"C:\\Program Files (x86)\\Microsoft Visual Studio")) {
                var msbuildPaths = _fileSystem.Directory.GetFiles(@"C:\\Program Files (x86)\\Microsoft Visual Studio", @"MSBuild.exe", SearchOption.AllDirectories);
                if (msbuildPaths.Any())
                {
                    var ordered = msbuildPaths.OrderByDescending(x => x.Substring(@"C:\\Program Files (x86)\\Microsoft Visual Studio\\".Length).Substring(0, 4)).ToList();
                    return ordered.First();
                }
            }
            
            // Else, find in default locations
            foreach(string possiblePath in fallbackLocations)
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
