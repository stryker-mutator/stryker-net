using System;
using Stryker.Core.Reporters.Html.RealTime.Events;

namespace Stryker.Core.Reporters.Html.RealTime;

public interface ISseServer
{
    public int Port { get; set; }
    bool HasConnectedClients { get; }

    event EventHandler<EventArgs> ClientConnected;

    public void OpenSseEndpoint();
    public void SendEvent<T>(SseEvent<T> @event);
    public void CloseSseEndpoint();
}
