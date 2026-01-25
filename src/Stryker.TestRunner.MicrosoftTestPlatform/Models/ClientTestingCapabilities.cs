using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record ClientTestingCapabilities(
    [property: JsonPropertyName("debuggerProvider")]
    bool DebuggerProvider);
