using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    public class TestClass
    {
        void TestMethod()
        {
            int i = 0;
            if (System.Environment.GetEnvironmentVariable("ActiveMutation") == "0")
            {
                i = i - 1;
            } else {
                i = i + 1;
            }
        }
    }
}
