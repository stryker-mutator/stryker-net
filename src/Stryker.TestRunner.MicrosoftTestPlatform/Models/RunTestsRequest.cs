using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

/// <summary>
/// Parameters of a <c>testing/runTests</c> request.
/// </summary>
/// <remarks>
/// The test selection is serialized as <c>tests</c> because that is the name the Microsoft Testing
/// Platform reads it under (<c>JsonRpcStrings.Tests</c>). The property is optional server-side, so a
/// misnamed selection is dropped silently and the server runs every test instead of the selection.
/// An absent selection is omitted rather than written as an explicit null, so that "run every test"
/// is expressed the way the protocol describes it.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record RunTestsRequest(
    [property:JsonPropertyName("runId")]
    Guid RunId,
    [property:JsonPropertyName("tests"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    TestNode[]? TestCases = null);
