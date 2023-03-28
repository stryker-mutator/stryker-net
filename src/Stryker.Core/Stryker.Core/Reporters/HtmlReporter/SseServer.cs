using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Stryker.Core.Mutants;
using Spectre.Console;

namespace Stryker.Core.Reporters.HtmlReporter;

public class SseServer
{
    private readonly IAnsiConsole _console;
    private readonly HttpListener _listener;
    private readonly List<StreamWriter> _writers;

    public SseServer(IAnsiConsole console)
    {
        _console = console;
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:8080/");
        _writers = new List<StreamWriter>();
    }

    public void Start()
    {
        _listener.Start();

        _console.WriteLine("Listening for SSE connections on http://localhost:8080/");

        // Start listening for new connections in a separate thread
        Task.Run(ListenForConnectionsAsync);
    }

    private async Task ListenForConnectionsAsync()
    {
        while (true)
        {
            var context = await _listener.GetContextAsync();
            var response = context.Response;
            response.ContentType = "text/event-stream";
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");

            var writer = new StreamWriter(response.OutputStream);
            _writers.Add(writer);
        }
    }

    public void PublishNewMutantData(IReadOnlyMutant result)
    {
        // Loop through connected clients and send the message to each writer
        foreach (var writer in _writers)
        {
            var mutationResult = new
            {
                Type = result.Mutation.Type.ToString(),
                Status = result.ResultStatus.ToString(),
            };
            writer.Write($"data: {JsonSerializer.Serialize(mutationResult)}\n\n");
            writer.Flush();
        }
    }

}

