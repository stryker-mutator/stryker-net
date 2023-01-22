using System;

namespace Stryker.Core.TestRunners.UnityTestRunner.RunUnity.UnityPath;

public class FailedToGetPathToUnityException : Exception
{
    public FailedToGetPathToUnityException(string message) : base(message)
    {
    }
}
