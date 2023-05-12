namespace Stryker.Core.Reporters.Html.Realtime;
using Stryker.Core.Reporters.Html.Realtime.Events;

public interface ISseServer
{
    public int Port { get; set; }

    public void OpenSseEndpoint();
    public void SendEvent<T>(SseEvent<T> @event);
    public void CloseSseEndpoint();
}
