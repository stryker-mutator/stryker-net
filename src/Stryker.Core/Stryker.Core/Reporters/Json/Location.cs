using System.Text.Json.Serialization;
using System.Text.Json;
using Stryker.Abstractions;
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

    public Location(MutationLocation location)
    {
        Start = new Position
        {
            Line = location.StartLine,
            Column = location.StartColumn
        };
        End = new Position
        {
            Line = location.EndLine,
            Column = location.EndColumn
        };
    }
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
