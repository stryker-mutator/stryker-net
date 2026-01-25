using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record RunRequest(
    [property:JsonPropertyName("runId")]
    Guid RunId);
