using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;

namespace Stryker.Core.TestRunners.UnityTestRunner.UnityPath;

public class UnityPath : IUnityPath
{
    private readonly IFileSystem _fileSystem;

    public UnityPath(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public string GetPath(string unityProjectPath)
    {
        var unityProjectDirectory = GetUnityProjectDirectory(unityProjectPath);
        var unityVersion = GetUnityVersion(unityProjectDirectory);

        var pathToUnity = string.Empty;
        // https://docs.unity3d.com/Manual/EditorCommandLineArguments.html
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            pathToUnity = @$"C:\Program Files\Unity\Hub\Editor\{unityVersion}\Editor\Unity.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            pathToUnity = @$"/Applications/Unity/Hub/Editor/{unityVersion}/Unity.app/Contents/MacOS/Unity";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            pathToUnity = @$"/Applications/Unity/Hub/Editor/{unityVersion}/Unity.app/Contents/Linux/Unity";
        }

        if (!string.IsNullOrEmpty(pathToUnity))
        {
            if (File.Exists(pathToUnity))
            {
                return pathToUnity;
            }

            throw new FailedToGetPathToUnityException(
                $"Unity wasn't found on default place '{pathToUnity}'. Please specify path to custom install location at your Stryker run settings");
        }

        throw new FailedToGetPathToUnityException(
            "Cannot find a Unity install location. Please specify path to custom install location at your Stryker run settings");
    }

    private string GetUnityProjectDirectory(string unityProjectPath)
    {
        var directoryInfo = _fileSystem.Directory.GetParent(unityProjectPath);
        if (directoryInfo.GetDirectories("Assets", SearchOption.TopDirectoryOnly).Any())
        {
            return directoryInfo.FullName;
        }

        if (_fileSystem.Directory.GetDirectories(unityProjectPath, "Assets", SearchOption.TopDirectoryOnly).Any())
        {
            return unityProjectPath;
        }

        throw new FailedToGetPathToUnityException("Wasn't found Unity project at path " + unityProjectPath);
    }

    private string GetUnityVersion(string unityProjectDirectory)
    {
        var pathToProjectVersionFile = Path.Combine(unityProjectDirectory, "ProjectSettings", "ProjectVersion.txt");
        var projectVersionContent =
            File.ReadAllLines(pathToProjectVersionFile);
        var lineWithProjectVersion = projectVersionContent.FirstOrDefault(line => line.Contains("m_EditorVersion"));
        if (string.IsNullOrEmpty(lineWithProjectVersion))
        {
            throw new FailedToGetPathToUnityException("m_EditorVersion wasn't found at " + pathToProjectVersionFile);
        }

        return lineWithProjectVersion.Split(" ").Last();
    }
}
