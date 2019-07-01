using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stryker.CLI.NuGet
{
    public class StrykerNugetFeedInfo
    {
        [JsonProperty("items")]
        private readonly List<NuGetLibraryItem> _items = null;

        public string LatestVersion { get; private set; }

        public static async Task<StrykerNugetFeedInfo> Create()
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var json = await httpclient.GetStringAsync("https://api.nuget.org/v3/registration3/dotnet-stryker/index.json");
                    var instance = JsonConvert.DeserializeObject<StrykerNugetFeedInfo>(json, Converter.Settings);
                    instance.LatestVersion = instance._items.First().Upper;

                    return instance;
                }
                catch
                {
                    return null;
                }
            }
        }

        public class NuGetLibraryItem
        {
            [JsonProperty("upper")]
            public string Upper { get; set; }
        }

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }
    }
}
