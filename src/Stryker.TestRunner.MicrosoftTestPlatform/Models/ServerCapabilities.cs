using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record ServerCapabilities(
    [property: JsonPropertyName("testing")]
    ServerTestingCapabilities Testing);
