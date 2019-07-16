using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.UnitTest.Mutants.TestResources
{
    class Mutator_Flag_MutatedStatics
    {
        private static bool willMutateToFalse = (StrykerNamespace.MutantControl.IsActive(0)?false:true);

        private static bool NoWorries => (StrykerNamespace.MutantControl.IsActive(1)?true:false);
        private static bool NoWorriesGetter
        {
            get {using(new StrykerNamespace.StaticContext()){ return (StrykerNamespace.MutantControl.IsActive(2)?true:false); }
}        }

static Mutator_Flag_MutatedStatics()
{using(new StrykerNamespace.StaticContext())        {
            int x = 0;
            var y = (StrykerNamespace.MutantControl.IsActive(3)?x--:x++);
        }
}    }
}
