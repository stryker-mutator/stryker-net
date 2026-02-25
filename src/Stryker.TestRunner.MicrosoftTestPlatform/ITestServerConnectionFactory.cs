using System.Net;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Abstracts the infrastructure needed to start a test server process and establish a JSON-RPC connection.
/// </summary>
internal interface ITestServerConnectionFactory
{
    /// <summary>
    /// Starts a TCP listener on an available port and returns the listener along with the assigned port.
    /// </summary>
    (ITestServerListener Listener, int Port) CreateListener();

    /// <summary>
    /// Starts the test server process for the given assembly, connecting back on the specified port.
    /// </summary>
    ITestServerProcess StartProcess(string assembly, int port, Dictionary<string, string?> environmentVariables);

    /// <summary>
    /// Creates an <see cref="ITestingPlatformClient"/> from an accepted TCP connection and process handle.
    /// When <paramref name="rpcLogFilePath"/> is non-null, all JSON-RPC frames are traced to that file.
    /// </summary>
    ITestingPlatformClient CreateClient(Stream stream, IProcessHandle processHandle, string? rpcLogFilePath);
}

/// <summary>
/// Wraps TCP listener operations for testability.
/// </summary>
internal interface ITestServerListener : IDisposable
{
    Task<(Stream Stream, IDisposable Connection)> AcceptConnectionAsync(CancellationToken cancellationToken);
    void Stop();
}

/// <summary>
/// Wraps the external test server process for testability.
/// </summary>
internal interface ITestServerProcess : IDisposable
{
    Task WaitForExitAsync();
    bool HasExited { get; }
    IProcessHandle ProcessHandle { get; }
}
