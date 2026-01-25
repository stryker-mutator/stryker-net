using System.Diagnostics;

namespace Stryker.TestRunner.MicrosoftTestPlatform.RPC;

internal sealed class ConsoleRpcListener : TraceListener
{
    public override void Write(string? message) => Console.Write(message ?? string.Empty);

    public override void WriteLine(string? message) => Console.WriteLine(message ?? string.Empty);
}
