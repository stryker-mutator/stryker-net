using System.Text.Json;

namespace Stryker.Core.Reporters.Html.Realtime.Events;

public class SseEvent<T>
{
    public string Event { get; init; }
    public T Data { get; init; }
}
