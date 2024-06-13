// https://github.com/stryker-mutator/stryker-net/issues/340

using System.Collections.Generic;
using System.Linq;

namespace TargetProject.Defects
{
    public class Linq
    {
        public void OrderByExample()
        {
            var someList = new List<int> { 1, 2, 3, 4, 5 };
            someList.OrderBy(i => i.ToString());
        }
    }
}
