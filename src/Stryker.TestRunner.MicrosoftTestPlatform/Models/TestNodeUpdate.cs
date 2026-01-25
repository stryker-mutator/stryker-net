using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record TestNodeUpdate
(
    [property: JsonPropertyName("node")]
    TestNode Node,

    [property: JsonPropertyName("parent")]
    string ParentUid);
