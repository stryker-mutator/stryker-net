using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record ServerTestingCapabilities(
    [property: JsonPropertyName("supportsDiscovery")]
    bool SupportsDiscovery,
    [property: JsonPropertyName("experimental_multiRequestSupport")]
    bool MultiRequestSupport,
    [property: JsonPropertyName("vsTestProvider")]
    bool VSTestProvider);
