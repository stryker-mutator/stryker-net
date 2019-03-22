using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    public class TestClass
    {
        void TestMethod()
        {
            int a = 1;
            int b = (Stryker.MutantControl.IsActive(1)?a -=1 + 2:a += (Stryker.MutantControl.IsActive(0)?1 -2:1 + 2));
        }
    }
}
