using System.Linq;

namespace Stryker.Core.UnitTest.Mutants.TestResources
{
    /// <summary>
    /// Hosts simple test to check mutators work properly
    /// </summary>
    class Basic
    {
        private void Linq()
        {
            var array = new []{1,2};

            var alt1 = (StrykerNamespace.MutantControl.IsActive(2)?array.Sum(x => x % 2 == 0):array.Count(x => (StrykerNamespace.MutantControl.IsActive(1)?x % 2 != 0:(StrykerNamespace.MutantControl.IsActive(0)?x * 2 :x % 2 )== 0)));
            var alt2 = (StrykerNamespace.MutantControl.IsActive(3)?array.Max():array.Min());
        }
    }
}