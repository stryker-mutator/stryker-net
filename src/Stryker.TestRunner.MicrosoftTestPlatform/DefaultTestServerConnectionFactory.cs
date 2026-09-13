using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using CliWrap;
using Microsoft.Extensions.Logging;
using Microsoft.Testing.Platform.ServerMode.Client;
using Stryker.Abstractions.Options;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Default implementation that creates TCP connections and starts processes via CliWrap.
/// </summary>
internal sealed class DefaultTestServerConnectionFactory : ITestServerConnectionFactory
{
    private readonly string? _outputPath;
    private readonly bool _logToFile;

    public DefaultTestServerConnectionFactory(IStrykerOptions? options = null)
    {
        _outputPath = options?.OutputPath;
        _logToFile = options?.LogOptions.LogToFile ?? false;
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
            .WithArguments([assembly, .. MtpServerConnector.BuildInProcessServerArguments(port)])
            .WithEnvironmentVariables(environmentVariables)
            .WithStandardOutputPipe(outputPipe)
            .WithStandardErrorPipe(outputPipe)
            .ExecuteAsync();

        return new CliTestServerProcess(cliProcess, outputStream);
    }

    public ITestingPlatformClient CreateClient(TcpClient tcpClient, IProcessHandle processHandle, ILogger logger)
    {
        var clientLogger = new DelegateMtpClientLogger((level, message) =>
        {
            var logLevel = level switch
            {
                MtpClientLogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
                MtpClientLogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
                MtpClientLogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
                MtpClientLogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                MtpClientLogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                _ => Microsoft.Extensions.Logging.LogLevel.Debug
            };

            logger.Log(logLevel, "{MtpClientMessage}", message);
        });

        var formatter = MtpServerConnector.CreateFormatter();
        var connection = MtpServerConnector.CreateConnection(tcpClient, formatter, clientLogger);
        var client = new MtpServerClient(connection, new MtpServerClientOptions
        {
            ClientName = "Stryker.NET",
            ClientVersion = typeof(TestingPlatformClient).Assembly.GetName().Version?.ToString() ?? "0.0.0",
            Logger = clientLogger
        });

        return new TestingPlatformClient(client, processHandle, logger);
    }

    private sealed class TcpTestServerListener(TcpListener listener) : ITestServerListener
    {
        public Task<TcpClient> AcceptConnectionAsync(CancellationToken cancellationToken)
            => listener.AcceptTcpClientAsync(cancellationToken).AsTask();

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
