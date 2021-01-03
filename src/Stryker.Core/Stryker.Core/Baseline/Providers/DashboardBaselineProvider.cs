using Stryker.Core.Clients;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System.Threading.Tasks;

namespace Stryker.Core.Baseline.Providers
{
    public class DashboardBaselineProvider : IBaselineProvider
    {
        private readonly IDashboardClient _client;
        public DashboardBaselineProvider(IStrykerOptions options, IDashboardClient client = null)
        {
            _client = client ?? new DashboardClient(options);
        }

        public async Task<JsonReport> Load(string version)
        {
            return await _client.PullReport(version);
        }

        public async Task Save(JsonReport report, string version)
        {
            await _client.PublishReport(report.ToJson(), version);
        }
    }
}
