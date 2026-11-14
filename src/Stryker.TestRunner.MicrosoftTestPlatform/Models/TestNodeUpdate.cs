using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record TestNodeUpdate(
    [property: JsonPropertyName("node")]
    TestNode Node,

    [property: JsonPropertyName("parent")]
    string ParentUid,

    // Time at which the test finished, used to place it on the timeline of the run: the stop time reported by
    // the test framework, or the time the update was received when the framework reports none
    [property: JsonIgnore]
    DateTime? FinishedAtUtc = null);
