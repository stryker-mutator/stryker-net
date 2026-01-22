using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record InitializeRequest(
    [property:JsonPropertyName("processId")]
    int ProcessId,

    [property:JsonPropertyName("clientInfo")]
    ClientInfo ClientInfo,

    [property:JsonPropertyName("capabilities")]
    ClientCapabilities Capabilities);
