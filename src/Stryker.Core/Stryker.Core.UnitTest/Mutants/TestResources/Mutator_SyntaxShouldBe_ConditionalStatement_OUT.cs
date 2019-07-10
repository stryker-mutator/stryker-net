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
            test = (StrykerNamespace.MutantControl.IsActive(0)?test -i:test + i);
            int testPlusTest = (StrykerNamespace.MutantControl.IsActive(1)?test -test:test + test);
            int b = (StrykerNamespace.MutantControl.IsActive(2)?a -=1 + 2:a += (StrykerNamespace.MutantControl.IsActive(3)?1 -2:1 + 2));
            var (one, two) = ((StrykerNamespace.MutantControl.IsActive(4)?1 -1:1 + 1), "");
            int Add(int x, int y) => (StrykerNamespace.MutantControl.IsActive(5)?x -y:x + y);
            Action act = () => Console.WriteLine((StrykerNamespace.MutantControl.IsActive(6)?1 -1:1 + 1), (StrykerNamespace.MutantControl.IsActive(7)?1 -1:1 + 1));
        }
    }
}