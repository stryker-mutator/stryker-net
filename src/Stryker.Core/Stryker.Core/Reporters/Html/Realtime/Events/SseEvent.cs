using System.Text.Json;

namespace Stryker.Core.Reporters.Html.Realtime.Events;

public class SseEvent<T>
{
    public string EventName { get; init; }
    public T Data { get; init; }

    public override string ToString()
    {
        var data = JsonSerializer.Serialize(Data,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return $"event: {EventName}\ndata:{data}\n\n";
    }
}
