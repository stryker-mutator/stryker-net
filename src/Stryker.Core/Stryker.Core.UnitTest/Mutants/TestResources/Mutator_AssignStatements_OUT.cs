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
            int b = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?a -=1 + 2:a += (StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?1 -2:1 + 2));
        }
    }
}
