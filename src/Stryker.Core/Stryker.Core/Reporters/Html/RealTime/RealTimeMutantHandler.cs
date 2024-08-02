using System;
using System.Collections.Generic;
using Stryker.Configuration.Mutants;
using Stryker.Configuration;
using Stryker.Configuration.Reporters.Html.RealTime.Events;
using Stryker.Configuration.Reporters.Json.SourceFiles;

namespace Stryker.Configuration.Reporters.Html.RealTime;

public class RealTimeMutantHandler : IRealTimeMutantHandler
{
    public int Port => _server.Port;

    private readonly ISseServer _server;
    private readonly Queue<JsonMutant> _delayedEventQueue = new();

    public RealTimeMutantHandler(StrykerOptions options, ISseServer server = null)
    {
        _server = server ?? new SseServer();
        _server.ClientConnected += ClientConnectedHandler;
    }

    public void OpenSseEndpoint() => _server.OpenSseEndpoint();

    public void CloseSseEndpoint()
    {
        _server.SendEvent(new SseEvent<string> { Event = SseEventType.Finished, Data = "" });
        _server.CloseSseEndpoint();
    }

    public void SendMutantTestedEvent(IReadOnlyMutant testedMutant)
    {
        var jsonMutant = new JsonMutant(testedMutant);

        if (_server.HasConnectedClients)
        {
            SendEvent(jsonMutant);
        }
        else
        {
            QueueJsonMutant(jsonMutant);
        }
    }

    private void SendEvent(JsonMutant jsonMutant)
    {
        _server.SendEvent(new SseEvent<JsonMutant> { Event = SseEventType.MutantTested, Data = jsonMutant });
    }

    private void QueueJsonMutant(JsonMutant jsonMutant)
    {
        _delayedEventQueue.Enqueue(jsonMutant);
    }

    private void ClientConnectedHandler(object sender, EventArgs e)
    {
        while (_delayedEventQueue.Count > 0)
        {
            SendEvent(_delayedEventQueue.Dequeue());
        }
    }
}
