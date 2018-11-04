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
            int testPlusTest = test + test;
            int b = a += 1 + 2;
            int Add(int x, int y) => x + y;
        }
    }
}
