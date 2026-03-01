using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Stryker.TestRunner.MicrosoftTestPlatform.RPC;

/// <summary>
/// Routes JSON-RPC trace output to a dedicated log file instead of the console.
/// One instance is created per <see cref="TestingPlatformClient"/> when log-to-file is enabled.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class FileRpcListener : TraceListener
{
    private readonly StreamWriter _writer;
    private readonly ILogger _logger;

    public FileRpcListener(string filePath, ILogger logger)
    {
        _logger = logger;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            _writer = new StreamWriter(filePath, append: false) { AutoFlush = true };
        }
        catch (Exception ex)
        {
            // Logging failure must not abort the test run; degrade to a no-op writer.
            _logger.LogWarning(ex, "Could not open/create RPC log file '{FilePath}'", filePath);
            _writer = StreamWriter.Null;
        }
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
