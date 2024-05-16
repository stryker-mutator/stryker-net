using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Stryker.Core.Reporters.Json;

internal static class JsonReportSerialization
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task<JsonReport> DeserializeJsonReportAsync(this Stream stream) => await JsonSerializer.DeserializeAsync<JsonReport>(stream, Options);

    public static async Task SerializeAsync(this JsonReport report, Stream stream) => await JsonSerializer.SerializeAsync(stream, report, Options);

    public static async Task<byte[]> SerializeAsync(this JsonReport report)
    {
        await using var stream = new MemoryStream();
        await report.SerializeAsync(stream);
        return stream.ToArray();
    }

    public static void Serialize(this JsonReport report, Stream stream)
    {
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = Options.WriteIndented });
        JsonSerializer.Serialize(writer, report, Options);
    }

    public static string ToJson(this JsonReport report) => JsonSerializer.Serialize(report, Options);

    public static string ToJsonHtmlSafe(this JsonReport report) => report.ToJson().Replace("<", "<\" + \"");
}
