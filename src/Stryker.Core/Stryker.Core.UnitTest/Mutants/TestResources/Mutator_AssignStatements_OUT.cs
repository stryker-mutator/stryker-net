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
            int b = Stryker.Helper.ActiveMutation==1? a += 1 - 2 : Stryker.Helper.ActiveMutation==0 ? a -= 1 + 2 : a += 1 + 2;
        }
    }
}
