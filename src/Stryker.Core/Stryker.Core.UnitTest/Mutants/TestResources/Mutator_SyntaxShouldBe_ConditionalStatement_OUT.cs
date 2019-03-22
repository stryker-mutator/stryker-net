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
            test = (Stryker.MutantControl.IsActive(0)?test -i:test + i);
            int testPlusTest = (Stryker.MutantControl.IsActive(1)?test -test:test + test);
            int b = (Stryker.MutantControl.IsActive(3)?a -=1 + 2:a += (Stryker.MutantControl.IsActive(2)?1 -2:1 + 2));
            var (one, two) = ((Stryker.MutantControl.IsActive(4)?1 -1:1 + 1), "");
            int Add(int x, int y) => (Stryker.MutantControl.IsActive(5)?x -y:x + y);
            Action act = () => Console.WriteLine((Stryker.MutantControl.IsActive(6)?1 -1:1 + 1), (Stryker.MutantControl.IsActive(7)?1 -1:1 + 1));
        }
    }
}
