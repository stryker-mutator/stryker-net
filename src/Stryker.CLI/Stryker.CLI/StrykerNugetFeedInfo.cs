using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Stryker.CLI.NuGet
{
    public class StrykerNugetFeedInfo
    {
        [JsonProperty("items")]
        private List<NuGetLibraryItem> _items = null;

        public string LatestVersion
        {
            get
            {
                return _items.First().Upper;
            }
        }

        public static StrykerNugetFeedInfo Create()
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var json = httpclient.GetStringAsync("https://api.nuget.org/v3/registration3/dotnet-stryker/index.json").Result;
                    return JsonConvert.DeserializeObject<StrykerNugetFeedInfo>(json, Converter.Settings);
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
