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
            int testPlusTest = System.Environment.GetEnvironmentVariable("ActiveMutation") == "0" ? test - test : test + test;
            int b = System.Environment.GetEnvironmentVariable("ActiveMutation") == "1" ? a += 1 - 2 : a += 1 + 2;
            int Add(int x, int y) => System.Environment.GetEnvironmentVariable("ActiveMutation") == "2" ? x - y : x + y;
        }
    }
}
