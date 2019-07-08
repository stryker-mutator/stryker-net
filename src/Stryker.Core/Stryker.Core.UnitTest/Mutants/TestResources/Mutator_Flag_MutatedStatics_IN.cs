using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.UnitTest.Mutants.TestResources
{
    class Mutator_Flag_MutatedStatics_IN
    {
        private static bool willMutateToFalse = true;

        private static bool NoWorries => false;

        static Mutator_Flag_MutatedStatics_IN()
        {
            int x = 0;
            var y = x++;
        }
    }
}
