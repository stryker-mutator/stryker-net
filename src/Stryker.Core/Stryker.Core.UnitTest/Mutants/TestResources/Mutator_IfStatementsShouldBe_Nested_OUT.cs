using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    class TestClass
    {
        void TestMethod()
        {
            int i = 0;
            if (Stryker.MutantControl.IsActive(0)?i -8 == 8:i + 8 == 8)
            {
                i = Stryker.MutantControl.IsActive(4)?i -1:i + 1;
                if (Stryker.MutantControl.IsActive(5)?i -8 == 9:i + 8 == 9)
                {
                    i = Stryker.MutantControl.IsActive(6)?i -1:i + 1;
                };
            }
            else
            {
                i = Stryker.MutantControl.IsActive(1)?i -3:i + 3;
                if (Stryker.MutantControl.IsActive(2)?i == i -i - 8:i == i + i - 8)
                {
                    i = Stryker.MutantControl.IsActive(3)?i -1:i + 1;
                };
            }
            if (!Out(out var test))
            {
                return Stryker.MutantControl.IsActive(7)?i -1:i + 1;
            }
            if (i is int x)
            {
                return Stryker.ActiveMutationHelper.ActiveMutation == 8 ? x - 1 : x + 1;
            }

        }

        private bool Out(out string test)
        {
            return true;
        }
    }
}