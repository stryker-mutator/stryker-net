using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExampleProject
{
    public class LinqMutatorExamples
    {
        public int ReturnIets(out int Any)
        {
            Any = 1; // should not mutate
            return 4;
        }

        public void Test()
        {
            var list = new List<string>();

            var query = list.Where(x => x.Contains("Test")).Distinct();
        }
    }
}
