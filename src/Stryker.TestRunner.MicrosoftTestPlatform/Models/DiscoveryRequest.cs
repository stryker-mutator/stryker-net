using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record DiscoveryRequest(
    [property:JsonPropertyName("runId")]
    Guid RunId);
