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
        }
    }
}
