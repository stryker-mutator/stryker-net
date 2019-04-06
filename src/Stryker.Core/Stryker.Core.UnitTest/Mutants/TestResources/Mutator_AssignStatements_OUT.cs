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
            int b = (StrykerNamespace.MutantControl.IsActive(1)?a -=1 + 2:a += (StrykerNamespace.MutantControl.IsActive(0)?1 -2:1 + 2));
        }
    }
}
