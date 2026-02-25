using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Stryker.TestRunner.MicrosoftTestPlatform.RPC;

/// <summary>
/// Routes JSON-RPC trace output to a dedicated log file instead of the console.
/// One instance is created per <see cref="TestingPlatformClient"/> when log-to-file is enabled.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class FileRpcListener : TraceListener
{
    private readonly StreamWriter _writer;

    public FileRpcListener(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        _writer = new StreamWriter(filePath, append: false) { AutoFlush = true };
    }

    public override void Write(string? message) => _writer.Write(message ?? string.Empty);

    public override void WriteLine(string? message) => _writer.WriteLine(message ?? string.Empty);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _writer.Dispose();
        }
        base.Dispose(disposing);
    }
}
