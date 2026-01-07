using System.Text.Json.Serialization;

namespace MsTestRunnerDemo.Models;

public sealed record ClientCapabilities(
    [property: JsonPropertyName("testing")]
    ClientTestingCapabilities Testing);
