using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record TestNode
(
    [property: JsonPropertyName("uid")]
    string Uid,

    [property: JsonPropertyName("display-name")]
    string DisplayName,

    [property: JsonPropertyName("node-type")]
    string NodeType,

    [property: JsonPropertyName("execution-state")]
    string ExecutionState,

    [property: JsonPropertyName("location.file")]
    string? LocationFile = null,

    [property: JsonPropertyName("location.line-start")]
    int? LocationLineStart = null,

    [property: JsonPropertyName("location.line-end")]
    int? LocationLineEnd = null,

    [property: JsonPropertyName("location.type")]
    string? LocationType = null,

    [property: JsonPropertyName("location.method")]
    string? LocationMethod = null);
