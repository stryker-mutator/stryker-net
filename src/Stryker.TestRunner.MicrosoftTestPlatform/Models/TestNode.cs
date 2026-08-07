using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

/// <summary>
/// A Microsoft Testing Platform test node.
/// </summary>
/// <remarks>
/// Nodes are received during discovery and sent back as the selection of a <c>testing/runTests</c>
/// request, so this type is serialized in both directions. The optional location properties are
/// omitted when absent rather than written as explicit nulls: the platform probes them with
/// <c>TryGetValue</c> and then asserts the value is not null, so an explicit null reads as a present
/// but invalid property and fails the request for every test the framework reported without a
/// location.
/// </remarks>
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

    [property: JsonPropertyName("location.file"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? LocationFile = null,

    [property: JsonPropertyName("location.line-start"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? LocationLineStart = null,

    [property: JsonPropertyName("location.line-end"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? LocationLineEnd = null,

    [property: JsonPropertyName("location.type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? LocationType = null,

    [property: JsonPropertyName("location.method"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? LocationMethod = null);
