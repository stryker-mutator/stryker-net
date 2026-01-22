using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record ServerCapabilities(
    [property: JsonPropertyName("testing")]
    ServerTestingCapabilities Testing);
