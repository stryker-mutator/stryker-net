using System;
using Stryker.Abstractions.Reporters.Html.RealTime.Events;

namespace Stryker.Abstractions.Reporters.Html.RealTime;

public interface ISseServer
{
    public int Port { get; set; }
    bool HasConnectedClients { get; }

    event EventHandler<EventArgs> ClientConnected;

    public void OpenSseEndpoint();
    public void SendEvent<T>(SseEvent<T> @event);
    public void CloseSseEndpoint();
}
