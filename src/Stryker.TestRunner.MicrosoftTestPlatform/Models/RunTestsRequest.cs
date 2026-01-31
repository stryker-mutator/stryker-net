using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record RunTestsRequest(
    [property:JsonPropertyName("runId")]
    Guid RunId,
    [property:JsonPropertyName("testCases")]
    TestNode[]? TestCases = null);
