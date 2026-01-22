using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record AttachDebuggerInfo(
    [property:JsonPropertyName("processId")]
    int ProcessId);
