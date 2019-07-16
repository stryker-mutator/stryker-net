using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.UnitTest.Mutants.TestResources
{
    class Mutator_Flag_MutatedStatics
    {
        private static bool willMutateToFalse = true;

        private static bool NoWorries => false;
        private static bool NoWorriesGetter
        {
            get { return false; }
        }

static Mutator_Flag_MutatedStatics()
        {
            int x = 0;
            var y = x++;
        }
    }
}
