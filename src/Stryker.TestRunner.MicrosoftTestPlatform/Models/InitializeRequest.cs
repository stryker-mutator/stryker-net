using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record InitializeRequest(
    [property:JsonPropertyName("processId")]
    int ProcessId,

    [property:JsonPropertyName("clientInfo")]
    ClientInfo ClientInfo,

    [property:JsonPropertyName("capabilities")]
    ClientCapabilities Capabilities);
