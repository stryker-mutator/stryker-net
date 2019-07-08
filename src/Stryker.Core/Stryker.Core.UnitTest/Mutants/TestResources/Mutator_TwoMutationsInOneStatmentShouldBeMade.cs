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
            if (i + 8 == 8 + i)
            {
                i--;
            }
        }
    }
}
