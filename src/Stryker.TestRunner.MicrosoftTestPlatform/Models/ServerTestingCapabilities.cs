using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record ServerTestingCapabilities(
    [property: JsonPropertyName("supportsDiscovery")]
    bool SupportsDiscovery,
    [property: JsonPropertyName("experimental_multiRequestSupport")]
    bool MultiRequestSupport,
    [property: JsonPropertyName("vsTestProvider")]
    bool VSTestProvider);
