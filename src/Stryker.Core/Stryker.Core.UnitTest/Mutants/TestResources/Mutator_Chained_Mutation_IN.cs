using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.UnitTest.Mutants.TestResources
{
    class MutatorChainedMutation
    {
        public void Simple()
        {
            object value = null;
            var flag1 = false;
            var flag2 = false;
            if (value != null && !flag1 && !flag2)
            {
            }
        }
    }
}