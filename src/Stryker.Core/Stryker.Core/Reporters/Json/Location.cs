using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Reporting;
using System;

namespace Stryker.Core.Reporters.Json;

public class Location : ILocation
{
    public IPosition Start { get; init; }
    public IPosition End { get; init; }

    public Location()
    {
    }

    public Location(FileLinePositionSpan location)
    {
        Start = new Position
        {
            Line = location.StartLinePosition.Line + 1,
            Column = location.StartLinePosition.Character + 1
        };
        End = new Position
        {
            Line = location.EndLinePosition.Line + 1,
            Column = location.EndLinePosition.Character + 1
        };
    }

    public bool Equals(ILocation other) => other is not null && Start.Equals(other.Start) && End.Equals(other.End);

    public override bool Equals(object obj) => obj is ILocation other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Start, End);
}

public class LocationConverter : JsonConverter<ILocation>
{
    public override ILocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Deserialize the JSON into the concrete type
        var location = JsonSerializer.Deserialize<Location>(ref reader, options);
        return location;
    }

    public override void Write(Utf8JsonWriter writer, ILocation value, JsonSerializerOptions options)
    {
        // Serialize the concrete type
        JsonSerializer.Serialize(writer, (Location)value, options);
    }
}
