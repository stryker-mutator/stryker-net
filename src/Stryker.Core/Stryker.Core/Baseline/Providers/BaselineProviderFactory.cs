using Stryker.Abstractions.Baseline;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;

namespace Stryker.Core.Baseline.Providers;

public static class BaselineProviderFactory
{
    public static IBaselineProvider Create(IStrykerOptions options)
    {
        return options.BaselineProvider switch
        {
            BaselineProvider.Dashboard => new DashboardBaselineProvider(options),
            BaselineProvider.Disk => new DiskBaselineProvider(options),
            BaselineProvider.AzureFileStorage => new AzureFileShareBaselineProvider(options),
            BaselineProvider.S3 => new S3BaselineProvider(options),
            _ => new DiskBaselineProvider(options),
        };
    }
}
