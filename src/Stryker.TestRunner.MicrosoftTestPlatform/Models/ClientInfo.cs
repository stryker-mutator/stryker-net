using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record ClientInfo(
    [property:JsonPropertyName("name")]
    string Name,

    [property:JsonPropertyName("version")]
    string Version = "1.0.0");
