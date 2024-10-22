using System.Threading.Tasks;
using Stryker.Abstractions;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Clients;

namespace Stryker.Core.Baseline.Providers;

public class DashboardBaselineProvider : IBaselineProvider
{
    private readonly IDashboardClient _client;
    public DashboardBaselineProvider(IStrykerOptions options, IDashboardClient client = null)
    {
        _client = client ?? new DashboardClient(options);
    }

    public async Task<IJsonReport> Load(string version)
    {
        return await _client.PullReport(version);
    }

    public async Task Save(IJsonReport report, string version)
    {
        await _client.PublishReport(report, version);
    }
}
