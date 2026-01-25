using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record ClientTestingCapabilities(
    [property: JsonPropertyName("debuggerProvider")]
    bool DebuggerProvider);
