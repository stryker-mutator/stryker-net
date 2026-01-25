using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public record TestNodeStateChangedEventArgs(
    [property: JsonPropertyName("runId")] Guid RunId,
    [property: JsonPropertyName("changes")] TestNodeUpdate[] Changes
    );
