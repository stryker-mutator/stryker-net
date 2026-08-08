using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Stryker.Utilities;

namespace Stryker.Core.Reporters.Json;

internal sealed class JsonReportDictionaryKeyConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
        typeToConvert.GetGenericArguments()[0] == typeof(string);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[1];
        return (JsonConverter)Activator.CreateInstance(
            typeof(JsonReportDictionaryKeyConverter<>).MakeGenericType(valueType));
    }
}

internal sealed class JsonReportDictionaryKeyConverter<TValue> : JsonConverter<IDictionary<string, TValue>>
{
    public override IDictionary<string, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var values = new Dictionary<string, TValue>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            var internalPath = FilePathUtils.NormalizePathSeparators(
                FilePathUtils.NormalizeReportPath(property.Name));
            values[internalPath] = property.Value.Deserialize<TValue>(options);
        }

        return values;
    }

    public override void Write(Utf8JsonWriter writer, IDictionary<string, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var (path, item) in value)
        {
            writer.WritePropertyName(FilePathUtils.NormalizeReportPath(path));
            JsonSerializer.Serialize(writer, item, options);
        }
        writer.WriteEndObject();
    }
}