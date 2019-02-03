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
                var test3 = Stryker.MutantControl.IsActive(0) ?2-5:2 + 5;
                return Stryker.MutantControl.IsActive(1) ?$"test{1 - test3}":$"test{1 + test3}";
            };
            Console.WriteLine(SomeLocalFunction());
        }
    }
}
