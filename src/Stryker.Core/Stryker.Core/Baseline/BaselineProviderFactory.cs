using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Baseline
{
    public static class BaselineProviderFactory
    {
        public static IBaselineProvider Create(StrykerOptions options)
        {
            return options.BaselineProvider switch
            {
                BaselineProvider.Dashboard => new DashboardBaselineProvider(options),
                BaselineProvider.Disk => new DiskBaselineProvider(options),
                _ => new DiskBaselineProvider(options),
            };
        }
    }
}
