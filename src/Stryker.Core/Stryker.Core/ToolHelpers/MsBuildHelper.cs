using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Testing;

namespace Stryker.Core.ToolHelpers;

public class MsBuildHelper
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private static readonly List<string> fallbackLocations =
    [
        @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe",
        @"C:\Windows\Microsoft.Net\Framework64\v4.0.30319\MSBuild.exe",
        @"C:\Windows\Microsoft.Net\Framework64\v3.5\MSBuild.exe",
        @"C:\Windows\Microsoft.Net\Framework64\v2.0.50727\MSBuild.exe",
        @"C:\Windows\Microsoft.Net\Framework\v4.0.30319\MSBuild.exe",
        @"C:\Windows\Microsoft.Net\Framework\v3.5\MSBuild.exe",
        @"C:\Windows\Microsoft.Net\Framework\v2.0.50727\MSBuild.exe"
    ];

    private string _msBuildPath;
    private readonly IProcessExecutor _executor;

    public MsBuildHelper(IFileSystem fileSystem = null, IProcessExecutor executor = null, string msBuildPath = null, ILogger logger = null)
    {
        _fileSystem = fileSystem ?? new FileSystem();
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<MsBuildHelper>();
        _executor = executor ?? new ProcessExecutor();
        _msBuildPath = msBuildPath;
    }


    public static string QuotesIfNeeded(string parameter)
    {
        if (!parameter.Contains(' ') || parameter.Length<3 || (parameter[0] == '"' && parameter[^1]=='"'))
        {
            return parameter;
        }
        return $"\"{parameter}\"";
    }

    public string GetVersion()
    {
        var (exe, command) = GetMsBuildExeAndCommand();
        var msBuildVersionOutput = _executor.Start("", exe, $"{command}-version /nologo");
        return msBuildVersionOutput.ExitCode != ExitCodes.Success ? string.Empty : msBuildVersionOutput.Output.Trim();
    }

    public string GetMsBuildPath()
    {
        if (!string.IsNullOrWhiteSpace(_msBuildPath))
        {
            return _msBuildPath;
        }
        // See if any MSBuild.exe can be found in visual studio installation folder
        _msBuildPath = SearchMsBuildVersion("latest") ?? SearchMsBuildVersion("prerelease");
        if (!string.IsNullOrWhiteSpace(_msBuildPath))
        {
            return _msBuildPath;
        }
        // Else, find in default locations
        _logger.LogInformation("Unable to find msbuild using vswhere, using fallback locations");

        _msBuildPath = fallbackLocations.Find(s => _fileSystem.File.Exists(s)) ?? throw new FileNotFoundException("MsBuild.exe could not be located. If you have MsBuild.exe available but still see this error please create an issue.");

        return _msBuildPath;
    }

    public (ProcessResult result, string exe, string command) BuildProject(string path, string projectFile, bool usingMsBuild, string configuration = null, string options = null)
    {
        var (exe, command) = usingMsBuild ? GetMsBuildExeAndCommand() : ("dotnet", "build");

        List<string> fullOptions =  string.IsNullOrEmpty(command) ? [QuotesIfNeeded(projectFile)] : [command, QuotesIfNeeded(projectFile)];
        if (!string.IsNullOrEmpty(configuration))
        {
            fullOptions.Add(usingMsBuild ? $"/property:Configuration={QuotesIfNeeded(configuration)}" : $"-c {QuotesIfNeeded(configuration)}");
        }

        if (options is not null)
        {
            fullOptions.Add(options);
        }

        var arguments = string.Join(' ', fullOptions);
        _logger.LogInformation("Building project {project} using {MsBuildPath} {Options} (directory {path}.)", projectFile, exe, arguments, path);
        return (_executor.Start(path, exe, arguments), exe, arguments);
    }

    private (string executable, string command) GetMsBuildExeAndCommand() => GetMsBuildPath().EndsWith(".exe", System.StringComparison.InvariantCultureIgnoreCase) ? (_msBuildPath, string.Empty) : ("dotnet", QuotesIfNeeded(_msBuildPath)+' ');

    private string SearchMsBuildVersion(string version)
    {
        foreach (var drive in Directory.GetLogicalDrives())
        {
            var visualStudioPath = Path.Combine(drive, "Program Files (x86)", "Microsoft Visual Studio");
            if (!_fileSystem.Directory.Exists(visualStudioPath)) continue;
            _logger.LogDebug("Using vswhere.exe to locate msbuild");

            var vsWherePath = Path.Combine(visualStudioPath, "Installer", "vswhere.exe");
            var vsWhereCommand =
                $@"-{version} -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe";
            var vsWhereResult = _executor.Start(visualStudioPath, vsWherePath, vsWhereCommand);

            if (vsWhereResult.ExitCode != ExitCodes.Success) continue;
            var msBuildPath = vsWhereResult.Output.Trim();
            if (!_fileSystem.File.Exists(msBuildPath)) continue;
            _logger.LogDebug("Msbuild executable path found at {MsBuildPath}",msBuildPath);

            return msBuildPath;
        }

        return null;
    }

}
