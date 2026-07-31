using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

/// <summary>
/// The `testing/runTests` request. Microsoft.Testing.Platform's server binds the
/// test filter from the `tests` property and each entry's `uid` and
/// `display-name`; a filter sent under any other property name is silently
/// ignored and the server runs the complete assembly.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record RunTestsRequest(
    [property: JsonPropertyName("runId")]
    Guid RunId,
    [property: JsonPropertyName("tests")]
    RunRequestTestNode[]? Tests = null);

[ExcludeFromCodeCoverage]
public sealed record RunRequestTestNode(
    [property: JsonPropertyName("uid")]
    string Uid,
    [property: JsonPropertyName("display-name")]
    string DisplayName);
