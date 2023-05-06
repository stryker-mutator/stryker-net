using System;

namespace Stryker.Core.Options;

// Optimization options 
[Flags]
public enum OptimizationModes
{
    None = 0,
    SkipUncoveredMutants = 1,
    CoverageBasedTest = 2,
    DisableBail = 4,
    CaptureCoveragePerTest = 8,
    DisableMixMutants = 16,
}
