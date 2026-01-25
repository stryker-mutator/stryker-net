using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public record TestNodeStateChangedEventArgs(
    [property: JsonPropertyName("runId")] Guid RunId,
    [property: JsonPropertyName("changes")] TestNodeUpdate[] Changes
    );
