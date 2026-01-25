using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record TelemetryPayload(
    [property: JsonPropertyName(nameof(TelemetryPayload.EventName))]
    string EventName,

    [property: JsonPropertyName("metrics")]
    IDictionary<string, object> Metrics);
