using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    public class TestClass
    {
        void TestMethod()
        {
            string SomeLocalFunction()
            {
                var test3 = Stryker.ActiveMutationHelper.ActiveMutation==0?2 -5:2 + 5;
                return $"test{Stryker.ActiveMutationHelper.ActiveMutation==1?1 -test3:1 + test3}";
            };
            Console.WriteLine(SomeLocalFunction());
        }
    }
}
