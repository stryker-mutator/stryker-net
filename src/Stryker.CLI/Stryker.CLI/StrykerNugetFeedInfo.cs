using Newtonsoft.Json;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stryker.CLI.NuGet
{
    public class StrykerNugetFeedInfo
    {
        [JsonProperty("versions")]
        private readonly List<string> _versions = default;

        public SemanticVersion LatestVersion
        {
            get
            {
                return _versions.Max(v => SemanticVersion.Parse(v));
            }
        }

        private StrykerNugetFeedInfo() { }
        public static async Task<StrykerNugetFeedInfo> Create()
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var json = await httpclient.GetStringAsync("https://api.nuget.org/v3-flatcontainer/dotnet-stryker/index.json");
                    var instance = JsonConvert.DeserializeObject<StrykerNugetFeedInfo>(json);
                    return instance;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
