using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.UnitTest.Mutants.TestResources
{
    class MutatorChainedMutation
    {
        public void Simple()
        {
            object value = null;
            var flag1 = (StrykerNamespace.MutantControl.IsActive(0)?true:false);
            var flag2 = (StrykerNamespace.MutantControl.IsActive(1)?true:false);
            if ((StrykerNamespace.MutantControl.IsActive(6)?value != null && !flag1 || !flag2:(StrykerNamespace.MutantControl.IsActive(4)?value != null || !flag1 :(StrykerNamespace.MutantControl.IsActive(2)?value == null :value != null )&& (StrykerNamespace.MutantControl.IsActive(3)?flag1 :!flag1 ))&& (StrykerNamespace.MutantControl.IsActive(5)?flag2:!flag2)))
            {
            }
        }
    }
}