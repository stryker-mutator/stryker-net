using System;

namespace Stryker.Core.Options
{
    // Optmisation options 
    [Flags]
    public enum OptimizationFlags
    {
        SkipUncoveredMutants,
        CoverageBasedTest,
        AbortTestOnKill
    }
}