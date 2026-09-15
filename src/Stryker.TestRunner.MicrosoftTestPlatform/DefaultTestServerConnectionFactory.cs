using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CliWrap;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.MicrosoftTestPlatform.RPC;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Default implementation that creates TCP connections and starts processes via CliWrap.
/// </summary>
internal sealed class DefaultTestServerConnectionFactory : ITestServerConnectionFactory
{
    private static readonly ConcurrentDictionary<string, Lazy<bool>> TestCaseFilterSupport =
        new(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

    private readonly string? _outputPath;
    private readonly bool _logToFile;
    private readonly string? _testCaseFilter;
    private readonly Func<string, bool> _supportsTestCaseFilter;

    public DefaultTestServerConnectionFactory(IStrykerOptions? options = null)
        : this(options, SupportsTestCaseFilter)
    {
    }

    internal DefaultTestServerConnectionFactory(IStrykerOptions? options, Func<string, bool> supportsTestCaseFilter)
    {
        ArgumentNullException.ThrowIfNull(supportsTestCaseFilter);

        _outputPath = options?.OutputPath;
        _logToFile = options?.LogOptions.LogToFile ?? false;
        _testCaseFilter = string.IsNullOrWhiteSpace(options?.TestCaseFilter) ? null : options.TestCaseFilter;
        _supportsTestCaseFilter = supportsTestCaseFilter;
    }

    public (ITestServerListener Listener, int Port) CreateListener()
    {
        var tcpListener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 0));
        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        return (new TcpTestServerListener(tcpListener), port);
    }

    public ITestServerProcess StartProcess(string assembly, int port, Dictionary<string, string?> environmentVariables)
    {
        var arguments = BuildArguments(assembly, port);
        Stream outputStream;
        PipeTarget outputPipe;

        if (_logToFile && !string.IsNullOrEmpty(_outputPath))
        {
            var logsDirectory = Path.Combine(_outputPath, "logs", "test-servers");
            Directory.CreateDirectory(logsDirectory);

            var logFileName = $"test-server-{Path.GetFileNameWithoutExtension(assembly)}-{port}-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.log";
            var logFilePath = Path.Combine(logsDirectory, logFileName);

            outputStream = File.Open(logFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            outputPipe = PipeTarget.ToStream(outputStream);
        }
        else
        {
            // Output is never consumed, so just discard it to avoid memory usage
            outputStream = Stream.Null;
            outputPipe = PipeTarget.Null;
        }

        var cliProcess = Cli.Wrap("dotnet")
            .WithWorkingDirectory(Path.GetDirectoryName(assembly) ?? string.Empty)
            .WithArguments(arguments)
            .WithEnvironmentVariables(environmentVariables)
            .WithStandardOutputPipe(outputPipe)
            .WithStandardErrorPipe(outputPipe)
            .ExecuteAsync();

        return new CliTestServerProcess(cliProcess, outputStream);
    }

    internal string[] BuildArguments(string assembly, int port)
    {
        List<string> arguments = [assembly, "--server", "--client-port", port.ToString()];

        if (_testCaseFilter is null)
        {
            return [.. arguments];
        }

        if (!_supportsTestCaseFilter(assembly))
        {
            throw new InputException(
                $"The test application '{Path.GetFileName(assembly)}' does not support 'test-case-filter' with Microsoft Testing Platform.",
                "Use a test framework version that registers the '--filter' option, or remove 'test-case-filter' from the Stryker configuration.");
        }

        arguments.AddRange(["--filter", _testCaseFilter]);
        return [.. arguments];
    }

    internal static bool HelpListsTestCaseFilter(string helpOutput) =>
        helpOutput
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimStart())
            .Any(line => line.Equals("--filter", StringComparison.Ordinal) ||
                         line.StartsWith("--filter ", StringComparison.Ordinal));

    private static bool SupportsTestCaseFilter(string assembly)
    {
        var fullAssemblyPath = Path.GetFullPath(assembly);
        return TestCaseFilterSupport
            .GetOrAdd(fullAssemblyPath, static path => CreateTestCaseFilterSupportProbe(path, ProbeTestCaseFilterSupport))
            .Value;
    }

    internal static Lazy<bool> CreateTestCaseFilterSupportProbe(string assembly, Func<string, bool> probe) =>
        new(() => probe(assembly), LazyThreadSafetyMode.ExecutionAndPublication);

    private static bool ProbeTestCaseFilterSupport(string assembly)
    {
        var standardOutput = new StringBuilder();
        var standardError = new StringBuilder();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            var helpProcess = Cli.Wrap("dotnet")
                .WithWorkingDirectory(Path.GetDirectoryName(assembly) ?? string.Empty)
                .WithArguments([assembly, "--help"])
                .WithValidation(CommandResultValidation.None)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(standardOutput))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(standardError))
                .ExecuteAsync(timeout.Token);

            helpProcess.Task.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
            throw new InputException(
                $"Timed out while checking whether test application '{Path.GetFileName(assembly)}' supports 'test-case-filter'.",
                "The test application did not complete '--help' within 30 seconds.");
        }
        catch (Exception ex)
        {
            throw new InputException(
                $"Could not determine whether test application '{Path.GetFileName(assembly)}' supports 'test-case-filter'.",
                $"{standardOutput}{standardError}{ex.Message}");
        }

        return HelpListsTestCaseFilter($"{standardOutput}{standardError}");
    }

    public ITestingPlatformClient CreateClient(Stream stream, IProcessHandle processHandle, ILogger logger, string? rpcLogFilePath)
    {
        var rpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter
        {
            JsonSerializerOptions = RpcJsonSerializerOptions.Default
        }));

        var tcpClient = new TcpClient();
        return new TestingPlatformClient(rpc, tcpClient, processHandle, logger, rpcLogFilePath);
    }

    private sealed class TcpTestServerListener(TcpListener listener) : ITestServerListener
    {
        public async Task<(Stream Stream, IDisposable Connection)> AcceptConnectionAsync(CancellationToken cancellationToken)
        {
            var tcpClient = await listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
            return (tcpClient.GetStream(), tcpClient);
        }

        public void Stop() => listener.Stop();

        public void Dispose() => listener.Stop();
    }

    private sealed class CliTestServerProcess(CommandTask<CommandResult> commandTask, Stream outputStream) : ITestServerProcess
    {
        private readonly ProcessHandle _processHandle = new(commandTask, outputStream);

        public Task WaitForExitAsync() => commandTask.Task;
        public bool HasExited => commandTask.Task.IsCompleted;
        public IProcessHandle ProcessHandle => _processHandle;

        public void Dispose()
        {
            _processHandle.Dispose();
            outputStream.Dispose();
        }
    }
}
