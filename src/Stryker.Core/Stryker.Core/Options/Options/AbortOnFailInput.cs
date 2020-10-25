using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options.Options
{
    class AbortOnFailInput : ComplexStrykerInput<OptimizationFlags, bool>
    {
        static AbortOnFailInput()
        {
            HelpText = @"Abort unit testrun as soon as any one unit test fails. This can reduce the overall running time.";
            DefaultInput = true;
            DefaultValue = new AbortOnFailInput(OptimizationsInput.DefaultValue, DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.AbortOnFail;

        public AbortOnFailInput(OptimizationFlags optimizationFlag, bool abortTestOnFail)
        {
            optimizationFlag |= abortTestOnFail ? OptimizationFlags.AbortTestOnKill : OptimizationFlags.NoOptimization;
            Value = optimizationFlag;
        }
    }
}
