using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record AttachDebuggerInfo(
    [property:JsonPropertyName("processId")]
    int ProcessId);
