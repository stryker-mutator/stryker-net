using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record TestNode
(
    [property: JsonPropertyName("uid")]
    string Uid,

    [property: JsonPropertyName("display-name")]
    string DisplayName,

    [property: JsonPropertyName("node-type")]
    string NodeType,

    [property: JsonPropertyName("execution-state")]
    string ExecutionState);
