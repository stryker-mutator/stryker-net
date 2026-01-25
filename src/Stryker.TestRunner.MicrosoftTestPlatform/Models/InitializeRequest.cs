using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record InitializeRequest(
    [property:JsonPropertyName("processId")]
    int ProcessId,

    [property:JsonPropertyName("clientInfo")]
    ClientInfo ClientInfo,

    [property:JsonPropertyName("capabilities")]
    ClientCapabilities Capabilities);
