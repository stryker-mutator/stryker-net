using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public record TestNodeStateChangedEventArgs(
    [property: JsonPropertyName("runId")] Guid RunId,
    [property: JsonPropertyName("changes")] TestNodeUpdate[] Changes
    );
