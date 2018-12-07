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
            test = Stryker.ActiveMutationHelper.ActiveMutation == 0 ? test - i : test + i;
            int testPlusTest = Stryker.ActiveMutationHelper.ActiveMutation == 1 ? test - test : test + test;
            int b = Stryker.ActiveMutationHelper.ActiveMutation == 3 ? a += 1 - 2 : Stryker.ActiveMutationHelper.ActiveMutation == 2 ? a -= 1 + 2 : a += 1 + 2;
            var (one, two) = Stryker.ActiveMutationHelper.ActiveMutation == 4 ? (1 - 1, "") : (1 + 1, "");
            int Add(int x, int y) => Stryker.ActiveMutationHelper.ActiveMutation == 5 ? x - y : x + y;
        }
    }
}