using System.Text.Json;

namespace Stryker.Core.Reporters.Html.RealTime.Events;

public class SseEvent<T>
{
    public SseEventType Event { get; init; }
    public T Data { get; init; }

    public string Serialize() => $@"
event:{Event.Serialize()}
data:{JsonSerializer.Serialize(Data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })}
";
}
