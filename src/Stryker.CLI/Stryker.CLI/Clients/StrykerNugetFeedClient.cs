using Newtonsoft.Json;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stryker.CLI.Clients
{
    public interface IStrykerNugetFeedClient
    {
        Task<SemanticVersion> GetMaxVersion();
    }

    public class StrykerNugetFeedClient : IStrykerNugetFeedClient
    {
        public class StrykerNugetFeed
        {
            [JsonProperty("versions")]
            public readonly List<string> Versions = default;
        }

        public async Task<SemanticVersion> GetMaxVersion()
        {
            using var httpclient = new HttpClient();
            try
            {
                var json = await httpclient.GetStringAsync("https://api.nuget.org/v3-flatcontainer/dotnet-stryker/index.json");
                var instance = JsonConvert.DeserializeObject<StrykerNugetFeed>(json);
                return instance.Versions.Max(v => SemanticVersion.Parse(v));
            }
            catch
            {
                return null;
            }
        }
    }
}
