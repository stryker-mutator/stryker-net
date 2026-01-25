using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public record TelemetryPayload
(
    [property: JsonPropertyName(nameof(TelemetryPayload.EventName))]
    string EventName,

    [property: JsonPropertyName("metrics")]
    IDictionary<string, object> Metrics);
