namespace Stryker.Core.TestRunners.UnityTestRunner.UnityPath;

public interface IUnityPath
{
    /// <summary>
    /// Get path to unity instance for the project at path
    /// </summary>
    /// <exception cref="FailedToGetPathToUnityException"></exception>
    /// <param name="unityProjectPath">Path to unity project. It may be and to root project directory and to any file on the top directory of Unity project
    /// For instance, .csproj or .sln files</param>
    /// <returns></returns>
    string GetPath(string unityProjectPath);
}
