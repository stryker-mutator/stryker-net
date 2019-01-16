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
            test = Stryker.ActiveMutationHelper.Check(0) ? test - i : test + i;
            int testPlusTest = Stryker.ActiveMutationHelper.Check(1) ? test - test : test + test;
            int b = Stryker.ActiveMutationHelper.Check(3) ? a += 1 - 2 : Stryker.ActiveMutationHelper.Check(2) ? a -= 1 + 2 : a += 1 + 2;
            var (one, two) = Stryker.ActiveMutationHelper.Check(4) ? (1 - 1, "") : (1 + 1, "");
            int Add(int x, int y) => Stryker.ActiveMutationHelper.Check(5) ? x - y : x + y;
            Action act = () => Console.WriteLine(Stryker.ActiveMutationHelper.Check(6) ? 1 - 1 : 1 + 1, Stryker.ActiveMutationHelper.Check(7) ? 1 - 1 : 1 + 1);
        }
    }
}