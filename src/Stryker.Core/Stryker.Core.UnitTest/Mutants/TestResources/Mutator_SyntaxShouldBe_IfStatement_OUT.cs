using System;
using System.Collections.Generic;
using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
{
    public class TestClass
    {
        void TestMethod()
        {
            if (Stryker.ActiveMutationHelper.ActiveMutation==1)
            {
                string SomeLocalFunction()
                {
                    var test3 = 2 + 5;
                    return $"test{1 - test3}";
                }
            }
            else
            {
                if (Stryker.ActiveMutationHelper.ActiveMutation==0)
                {
                    string SomeLocalFunction()
                    {
                        var test3 = 2 - 5;
                        return $"test{1 + test3}";
                    }
                }
                else
                {
                    string SomeLocalFunction()
                    {
                        var test3 = 2 + 5;
                        return $"test{1 + test3}";
                    }
                }
            };
            Console.WriteLine(SomeLocalFunction());
        }
    }
}
