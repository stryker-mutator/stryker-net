﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Stryker.Core.Reporters.Html.Realtime.Events;

namespace Stryker.Core.Reporters.Html.Realtime;

public class SseServer : ISseServer
{
    public int Port { get; set; }

    private readonly HttpListener _listener;
    private readonly List<StreamWriter> _writers;

    public SseServer()
    {
        Port = FreeTcpPort();

        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{Port}/");
        _writers = new List<StreamWriter>();
    }

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
        _listener.Start();

        Task.Run(ListenForConnectionsAsync);
    }

    private async Task ListenForConnectionsAsync()
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
            _writers.Add(writer);
        }
    }

    public void SendEvent<T>(SseEvent<T> @event)
    {
        var serialized = @event.Serialize();
        foreach (var writer in _writers)
        {
            writer.Write($"{serialized}{Environment.NewLine}{Environment.NewLine}");
            writer.Flush();
        }
    }

    public void CloseSseEndpoint()
    {
        Task.WaitAll(_writers.Select(writer => writer.BaseStream.FlushAsync()).ToArray());

        _listener.Close();
    }
}
