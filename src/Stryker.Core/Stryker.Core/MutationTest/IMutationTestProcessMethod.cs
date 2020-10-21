using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.MutationTest
{
    interface IMutationTestProcessMethod
    {
        public void Mutate();

        public void FilterMutants();
    }
}
