using System;

namespace Stryker.Core.Options
{
    // Optimization options 
    [Flags]
    public enum OptimizationFlags
    {
        NoOptimization = 0,
        SkipUncoveredMutants = 1,
        CoverageBasedTest = 2,
        AbortTestOnKill = 4,
        CaptureCoveragePerTest = 8,
        UseEnvVariable = 16
    }
}