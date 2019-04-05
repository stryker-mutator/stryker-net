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
            test = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?test -i:test + i);
            int testPlusTest = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?test -test:test + test);
            int b = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==3?a -=1 + 2:a += (StrykerNamespace.ActiveMutationHelper.ActiveMutation==2?1 -2:1 + 2));
            var (one, two) = ((StrykerNamespace.ActiveMutationHelper.ActiveMutation==4?1 -1:1 + 1), "");
            int Add(int x, int y) => (StrykerNamespace.ActiveMutationHelper.ActiveMutation==5?x -y:x + y);
            Action act = () => Console.WriteLine((StrykerNamespace.ActiveMutationHelper.ActiveMutation==6?1 -1:1 + 1), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==7?1 -1:1 + 1));
        }
    }
}
