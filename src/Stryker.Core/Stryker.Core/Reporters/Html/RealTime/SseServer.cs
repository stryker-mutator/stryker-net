using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Stryker.Core.Reporters.Html.RealTime.Events;

namespace Stryker.Core.Reporters.Html.RealTime;

public class SseServer : ISseServer, IDisposable
{
    public int Port { get; set; }
    public bool HasConnectedClients
    {
        get
        {
            lock (_writersLock)
            {
                return _writers.Count > 0;
            }
        }
    }

    private readonly HttpListener _listener;
    private readonly List<StreamWriter> _writers;
    private readonly object _writersLock = new();
    private readonly TaskCompletionSource<bool> _disposeCompletion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool _disposedValue;

    public SseServer()
    {
        Port = FreeTcpPort();

        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{Port}/");
        _writers = new List<StreamWriter>();
    }

    public int ConnectedClients
    {
        get
        {
            lock (_writersLock)
            {
                return _writers.Count;
            }
        }
    }

    public event EventHandler<EventArgs> ClientConnected;

    private static int FreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public void OpenSseEndpoint()
    {
        lock (_writersLock)
        {
            ObjectDisposedException.ThrowIf(_disposedValue, this);
            _listener.Start();
            Task.Run(ListenForConnectionsAsync);
        }
    }

    private async Task ListenForConnectionsAsync()
    {
        try
        {
            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();
                var response = context.Response;
                response.ContentType = "text/event-stream";
                // The file:// protocols needs this, since we can't add a file location as an allowed origin.
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                response.Headers.Add("Cache-Control", "no-cache");
                response.Headers.Add("Connection", "keep-alive");

                var writer = new StreamWriter(response.OutputStream);
                lock (_writersLock)
                {
                    if (_disposedValue)
                    {
                        DisposeWriter(writer);
                        return;
                    }

                    _writers.Add(writer);
                }

                ClientConnected?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (HttpListenerException)
        {
            // The listener was closed while waiting for the next client.
        }
        catch (ObjectDisposedException)
        {
            // The listener was disposed while waiting for the next client.
        }
    }

    public void SendEvent<T>(SseEvent<T> @event)
    {
        var serialized = @event.Serialize();
        lock (_writersLock)
        {
            if (_disposedValue)
            {
                return;
            }

            var lostClients = new List<StreamWriter>();
            foreach (var writer in _writers)
            {
                try
                {
                    writer.Write($"{serialized}{Environment.NewLine}{Environment.NewLine}");
                    writer.Flush();
                }
                catch (HttpListenerException)
                {
                    // The client disconnected
                    lostClients.Add(writer);
                }
                catch (IOException)
                {
                    // The client disconnected while the event was being written.
                    lostClients.Add(writer);
                }
                catch (ObjectDisposedException)
                {
                    // The response stream was disposed while the event was being written.
                    lostClients.Add(writer);
                }
            }

            foreach (var lostClient in lostClients)
            {
                _writers.Remove(lostClient);
                DisposeWriter(lostClient);
            }
        }
    }

    public void CloseSseEndpoint() => Dispose();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        var ownsDisposal = false;
        lock (_writersLock)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
                ownsDisposal = true;
            }
        }

        if (!ownsDisposal)
        {
            _disposeCompletion.Task.GetAwaiter().GetResult();
            return;
        }

        try
        {
            _listener.Close();
            List<StreamWriter> writers;
            lock (_writersLock)
            {
                writers = [.. _writers];
                _writers.Clear();
            }

            foreach (var writer in writers)
            {
                DisposeWriter(writer);
            }
        }
        finally
        {
            _disposeCompletion.TrySetResult(true);
        }
    }

    private static void DisposeWriter(StreamWriter writer)
    {
        try
        {
            writer.Dispose();
        }
        catch (HttpListenerException)
        {
            // The client disconnected before the writer was disposed.
        }
        catch (IOException)
        {
            // Flushing the writer failed because the client disconnected.
        }
        catch (ObjectDisposedException)
        {
            // Closing the listener already disposed the response stream.
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
