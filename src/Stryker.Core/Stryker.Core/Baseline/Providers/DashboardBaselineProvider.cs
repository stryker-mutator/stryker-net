using Stryker.Configuration.Clients;
using Stryker.Configuration;
using Stryker.Configuration.Reporters.Json;
using System.Threading.Tasks;

namespace Stryker.Configuration.Baseline.Providers
{
    public class DashboardBaselineProvider : IBaselineProvider
    {
        private readonly IDashboardClient _client;
        public DashboardBaselineProvider(StrykerOptions options, IDashboardClient client = null)
        {
            _client = client ?? new DashboardClient(options);
        }

        public async Task<JsonReport> Load(string version)
        {
            return await _client.PullReport(version);
        }

        public async Task Save(JsonReport report, string version)
        {
            await _client.PublishReport(report, version);
        }
    }
}
