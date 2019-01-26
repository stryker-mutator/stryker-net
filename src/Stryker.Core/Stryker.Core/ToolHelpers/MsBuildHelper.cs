using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.ToolHelpers
{
    public class MsBuildHelper
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<string> possibleLocations;

        public MsBuildHelper(IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            possibleLocations = new List<string>
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
            foreach(string possiblePath in possibleLocations)
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
