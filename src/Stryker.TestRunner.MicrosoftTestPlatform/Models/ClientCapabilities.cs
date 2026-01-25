using System.Text.Json.Serialization;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record ClientCapabilities(
    [property: JsonPropertyName("testing")]
    ClientTestingCapabilities Testing);
