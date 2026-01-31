using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record ServerTestingCapabilities(
    [property: JsonPropertyName("supportsDiscovery")]
    bool SupportsDiscovery,
    [property: JsonPropertyName("experimental_multiRequestSupport")]
    bool MultiRequestSupport,
    [property: JsonPropertyName("vsTestProvider")]
    bool VSTestProvider);
