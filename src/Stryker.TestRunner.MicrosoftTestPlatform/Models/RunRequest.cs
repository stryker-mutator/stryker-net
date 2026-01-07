using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record RunRequest(
    [property:JsonPropertyName("runId")]
    Guid RunId);
