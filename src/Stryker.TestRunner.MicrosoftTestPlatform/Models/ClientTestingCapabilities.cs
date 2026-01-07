using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record ClientTestingCapabilities(
    [property: JsonPropertyName("debuggerProvider")]
    bool DebuggerProvider);
