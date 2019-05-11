using System;

namespace Stryker.Core.Options
{
    // Optimization options 
    [Flags]
    public enum OptimizationFlags
    {
        SkipUncoveredMutants,
        CoverageBasedTest,
        AbortTestOnKill
    }
}