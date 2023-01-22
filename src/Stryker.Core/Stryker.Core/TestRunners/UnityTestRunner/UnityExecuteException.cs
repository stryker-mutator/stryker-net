using System;

namespace Stryker.Core.TestRunners.UnityTestRunner;

public class UnityExecuteException : Exception
{
    public int ExitCode;
    public string PathToUnityLogFile;

    public UnityExecuteException(int exitCode, string pathToUnityLogFile)
    {
        ExitCode = exitCode;
        PathToUnityLogFile = pathToUnityLogFile;
    }

    public override string ToString() => $"Exit code : {ExitCode}. Details in log file {PathToUnityLogFile}{Environment.NewLine}" + base.ToString();
}
