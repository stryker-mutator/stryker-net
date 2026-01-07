// Stryker.Mtp.Orchestrator
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public sealed class MtpJsonRpcConnection : IAsyncDisposable
{
    readonly TcpListener _listener;
    TcpClient? _client;
    Stream? _io;
    int _nextId = 1;

    public int Port { get; }

    public MtpJsonRpcConnection()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
    }

    public async Task AcceptAsync(CancellationToken ct)
    {
        _client = await _listener.AcceptTcpClientAsync(ct);
        _io = _client.GetStream();
        _ = Task.Run(() => PumpNotificationsAsync(ct)); // read async notifications
    }

    public async Task<JsonElement?> RequestAsync(string method, object? @params, CancellationToken ct)
    {
        var id = Interlocked.Increment(ref _nextId);
        var payload = JsonSerializer.SerializeToUtf8Bytes(new
        {
            jsonrpc = "2.0",
            id,
            method,
            @params
        });
        await SendAsync(payload, ct);
        return await ReadResponseAsync(id, ct);
    }

    async Task SendAsync(byte[] payload, CancellationToken ct)
    {
        // JSON-RPC over TCP with Content-Length framing
        var header = Encoding.ASCII.GetBytes($"Content-Length: {payload.Length}\r\n\r\n");
        await _io!.WriteAsync(header, ct);
        await _io!.WriteAsync(payload, ct);
        await _io!.FlushAsync(ct);
    }

    async Task<JsonElement?> ReadResponseAsync(int id, CancellationToken ct)
    {
        // Very small, blocking parser for demo. Replace with robust framing/parser in production.
        using var ms = new MemoryStream();
        var buffer = new byte[8192];
        // Read until we get a JSON with the matching id.
        while (true)
        {
            int n = await _io!.ReadAsync(buffer, ct);
            if (n == 0) throw new IOException("Disconnected");
            ms.Write(buffer, 0, n);
            var str = Encoding.UTF8.GetString(ms.ToArray());
            int idx = str.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (idx >= 0)
            {
                var json = str[(idx + 4)..];
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("id", out var got) && got.GetInt32() == id)
                {
                    return doc.RootElement.TryGetProperty("result", out var res) ? res : null;
                }
            }
        }
    }

    async Task PumpNotificationsAsync(CancellationToken ct)
    {
        // Parse streaming notifications like testing/testUpdates/tests and client/log.
        var reader = new StreamReader(_io!, Encoding.UTF8, leaveOpen: true);
        while (!ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line is null) break;
            if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
            {
                var len = int.Parse(line.Split(':')[1].Trim());
                await reader.ReadLineAsync(ct); // empty line
                var buf = new char[len];
                await reader.ReadBlockAsync(buf.AsMemory(), ct);
                var json = new string(buf);
                HandleNotification(json);
            }
        }
    }

    void HandleNotification(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("method", out var m))
        {
            var method = m.GetString();
            if (method == "testing/testUpdates/tests")
            {
                // TODO: update Stryker test result cache
            }
            else if (method == "testing/testUpdates/attachments")
            {
                // TODO: handle attachments
            }
            else if (method == "client/log")
            {
                var p = root.GetProperty("params");
                var text = p.GetProperty("message").GetString();
                if (text!.StartsWith("STRYKER_COV:", StringComparison.Ordinal))
                {
                    // parse JSON after prefix → test->mutant hits
                }
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        try { _io?.Dispose(); } catch { }
        try { _client?.Dispose(); } catch { }
        try { _listener.Stop(); } catch { }
        await Task.CompletedTask;
    }
}
