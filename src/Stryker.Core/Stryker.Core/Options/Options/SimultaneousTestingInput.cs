using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options.Options
{
    class SimultaneousTestingInput : ComplexStrykerInput<OptimizationFlags, bool>
    {
        static SimultaneousTestingInput()
        {
            HelpText = @"Test each mutation in an isolated test run.";
            DefaultInput = false;
            DefaultValue = new SimultaneousTestingInput(AbortOnFailInput.DefaultValue, DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.SimultaneousTesting;

        public SimultaneousTestingInput(OptimizationFlags optimizationFlag, bool disableSimultaneousTesting)
        {
            optimizationFlag |= disableSimultaneousTesting ? OptimizationFlags.DisableTestMix : OptimizationFlags.NoOptimization;
            Value = optimizationFlag;
        }
    }
}
