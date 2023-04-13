using System.Text.Json;

namespace Stryker.Core.Reporters.Html.Realtime.Events;

public class SseEvent<T>
{
    public SseEventType Type { get; init; }
    public T Data { get; init; }

    public override string ToString()
    {
        var @event = Type.ToString().ToLower();
        var data = JsonSerializer.Serialize(Data,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return $"event: {@event}\ndata:{data}\n\n";
    }
}
