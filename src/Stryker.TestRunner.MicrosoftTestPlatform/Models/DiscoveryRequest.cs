using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record DiscoveryRequest(
    [property:JsonPropertyName("runId")]
    Guid RunId);
