using Newtonsoft.Json;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stryker.CLI.Clients
{
    public interface IStrykerNugetFeedClient
    {
        Task<SemanticVersion> GetLatestVersionAsync();
        Task<SemanticVersion> GetPreviewVersionAsync();
    }

    public class StrykerNugetFeedClient : IStrykerNugetFeedClient
    {
        private const string _nugetStrykerFeed = "https://api.nuget.org/v3-flatcontainer/dotnet-stryker/index.json";
        private StrykerNugetFeed _instance;

        public class StrykerNugetFeed
        {
            [JsonProperty("versions")]
            public readonly List<string> Versions = default;
        }

        public async Task<SemanticVersion> GetLatestVersionAsync()
        {
            var versions = await GetVersionsAsync();

            // Prereleases shouldn't show as an available update
            var latestVersion = versions.Where(x => !x.IsPrerelease).Max();
            return latestVersion;
        }

        public async Task<SemanticVersion> GetPreviewVersionAsync()
        {
            var versions = await GetVersionsAsync();

            var latestPreviewVersion = versions.Max();
            return latestPreviewVersion;
        }

        private async Task<IEnumerable<SemanticVersion>> GetVersionsAsync()
        {
            _instance ??= await GetFeedAsync();

            return _instance?.Versions?.Select(v => SemanticVersion.Parse(v)) ?? new[] { new SemanticVersion(0, 0, 0) };
        }

        private async Task<StrykerNugetFeed> GetFeedAsync()
        {
            using var httpclient = new HttpClient();
            try
            {
                var json = await httpclient.GetStringAsync(_nugetStrykerFeed);
                return JsonConvert.DeserializeObject<StrykerNugetFeed>(json);
            }
            catch { return null; }
        }
    }
}
