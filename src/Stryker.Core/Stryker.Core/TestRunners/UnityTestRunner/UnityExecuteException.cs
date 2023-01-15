using System;

namespace Stryker.Core.TestRunners.UnityTestRunner;

public class UnityExecuteException : Exception
{
    public int ExitCode;
    public string Output;

    public UnityExecuteException(int exitCode, string output)
    {
        ExitCode = exitCode;
        Output = output;
    }

    public override string ToString() => $"Exit code : {ExitCode}{Environment.NewLine}" + base.ToString();
}
