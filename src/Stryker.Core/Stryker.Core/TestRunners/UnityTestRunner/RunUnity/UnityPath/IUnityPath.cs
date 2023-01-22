using Stryker.Core.Options;

namespace Stryker.Core.TestRunners.UnityTestRunner.RunUnity.UnityPath;

public interface IUnityPath
{
    /// <summary>
    /// Get path to unity instance for the project at path
    /// </summary>
    /// <exception cref="FailedToGetPathToUnityException"></exception>
    string GetPath(StrykerOptions options);
}
