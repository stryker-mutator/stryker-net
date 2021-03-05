using Stryker.Core.Options;

namespace Stryker.Core.Baseline.Providers
{
    public static class BaselineProviderFactory
    {
        public static IBaselineProvider Create(IStrykerOptions options)
        {
            return options.BaselineProvider switch
            {
                BaselineProvider.Dashboard => new DashboardBaselineProvider(options),
                BaselineProvider.Disk => new DiskBaselineProvider(options),
                BaselineProvider.AzureFileStorage => new AzureFileShareBaselineProvider(options),
                _ => new DiskBaselineProvider(options),
            };
        }
    }
}
