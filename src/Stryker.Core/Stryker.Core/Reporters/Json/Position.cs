using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Stryker.Abstractions.Reporting;

namespace Stryker.Core.Reporters.Json;

public class Position : IPosition
{
    private int _line;
    public int Line
    {
        get => _line;
        set => _line = value > 0 ? value : throw new ArgumentException("Line number must be higher than 0");
    }

    private int _column;
    public int Column
    {
        get => _column;
        set => _column = value > 0 ? value : throw new ArgumentException("Column number must be higher than 0");
    }
}

public class PositionConverter : JsonConverter<IPosition>
{
    public override IPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Deserialize the JSON into the concrete type
        var position = JsonSerializer.Deserialize<Position>(ref reader, options);
        return position;
    }

    public override void Write(Utf8JsonWriter writer, IPosition value, JsonSerializerOptions options)
    {
        // Serialize the concrete type
        JsonSerializer.Serialize(writer, (Position)value, options);
    }
}
