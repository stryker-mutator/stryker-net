
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Stryker.Abstractions.Reporting;

namespace Stryker.Core.Reporters.Json.TestFiles
{
    public sealed class JsonTest : IEquatable<IJsonTest>, IJsonTest
    {
        public string Id { get; }
        public string Name { get; set; }
        public ILocation Location { get; set; }

        public JsonTest(string id) => Id = id;

        public bool Equals(IJsonTest other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((JsonTest)obj);
        }

        public override int GetHashCode() => Id != null ? Id.GetHashCode() : 0;

        public static bool operator ==(JsonTest left, JsonTest right) => Equals(left, right);

        public static bool operator !=(JsonTest left, JsonTest right) => !Equals(left, right);

    }

    public class JsonTestConverter : JsonConverter<IJsonTest>
    {
        public override IJsonTest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Deserialize the JSON into the concrete type
            var jsonTest = JsonSerializer.Deserialize<JsonTest>(ref reader, options);
            return jsonTest;
        }

        public override void Write(Utf8JsonWriter writer, IJsonTest value, JsonSerializerOptions options)
        {
            // Serialize the concrete type
            JsonSerializer.Serialize(writer, (JsonTest)value, options);
        }
    }
}
