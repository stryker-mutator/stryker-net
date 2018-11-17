using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    public class TestClass
    {
        void TestMethod()
        {
            int test = 10;
            int testPlusTest = Stryker.Environment.ActiveMutation==0 ? test - test : test + test;
            int b = Stryker.Environment.ActiveMutation==2 ? a += 1 - 2 : Stryker.Environment.ActiveMutation==1 ? a -= 1 + 2 : a += 1 + 2;
            int Add(int x, int y) => Stryker.Environment.ActiveMutation==3 ? x - y : x + y;
        }
    }
}
