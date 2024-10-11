using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.Reporters.Json.TestFiles;

namespace Stryker.Core.Reporters.Json
{
    internal static class JsonReportSerialization
    {
        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new SourceFileConverter(), new JsonMutantConverter(), new LocationConverter(), new PositionConverter(), new JsonTestFileConverter(), new JsonTestConverter() },
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static async Task<IJsonReport> DeserializeJsonReportAsync(this Stream stream) => await JsonSerializer.DeserializeAsync<JsonReport>(stream, Options);

        public static async Task SerializeAsync(this IJsonReport report, Stream stream) => await JsonSerializer.SerializeAsync(stream, report, Options);

        public static async Task<byte[]> SerializeAsync(this IJsonReport report)
        {
            await using var stream = new MemoryStream();
            await report.SerializeAsync(stream);
            return stream.ToArray();
        }

        public static void Serialize(this IJsonReport report, Stream stream)
        {
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = Options.WriteIndented });
            JsonSerializer.Serialize(writer, report, Options);
        }

        public static string ToJson(this IJsonReport report) => JsonSerializer.Serialize(report, Options);

        public static string ToJsonHtmlSafe(this IJsonReport report) => report.ToJson().Replace("<", "<\" + \"");
    }
}
