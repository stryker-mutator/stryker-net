using System.Diagnostics;

namespace MsTestRunnerDemo;

internal sealed class ConsoleRpcListener : TraceListener
{
    public override void Write(string? message) => Console.Write(message ?? string.Empty);

    public override void WriteLine(string? message) => Console.WriteLine(message ?? string.Empty);
}
