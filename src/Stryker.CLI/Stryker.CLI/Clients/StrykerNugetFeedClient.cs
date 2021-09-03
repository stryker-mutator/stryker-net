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
        private const string NugetStrykerFeed = "https://api.nuget.org/v3-flatcontainer/dotnet-stryker/index.json";

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
                var json = await httpclient.GetStringAsync(NugetStrykerFeed);
                var instance = JsonConvert.DeserializeObject<StrykerNugetFeed>(json);

                // Prereleases shouldn't show as an available update
                var versions = instance.Versions.Select(v => SemanticVersion.Parse(v)).Where(x => !x.IsPrerelease);
                return versions.Max();
            }
            catch
            {
                return new SemanticVersion(0, 0, 0);
            }
        }
    }
}
