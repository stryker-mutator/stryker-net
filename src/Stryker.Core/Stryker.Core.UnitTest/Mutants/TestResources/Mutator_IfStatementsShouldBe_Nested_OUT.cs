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
            if ((StrykerNamespace.MutantControl.IsActive(0)?i -8 :i + 8 )== 8)
            {
                i = (StrykerNamespace.MutantControl.IsActive(4)?i -1:i + 1);
                if ((StrykerNamespace.MutantControl.IsActive(5)?i -8 :i + 8 )== 9)
                {
                    i = (StrykerNamespace.MutantControl.IsActive(6)?i -1:i + 1);
                };
            }
            else
            {
                i = (StrykerNamespace.MutantControl.IsActive(1)?i -3:i + 3);
                if (i == (StrykerNamespace.MutantControl.IsActive(2)?i -i :i + i )- 8)
                {
                    i = (StrykerNamespace.MutantControl.IsActive(3)?i -1:i + 1);
                };
            }

            if (!Out(out var test))
            {
                return (StrykerNamespace.MutantControl.IsActive(7)?i -1:i + 1);
            }

            if (i is int x)
            {
                return (StrykerNamespace.MutantControl.IsActive(8)?x -1:x + 1);
            }
        }

        private bool Out(out string test)
        {
            return true;
        }
    }
}
,