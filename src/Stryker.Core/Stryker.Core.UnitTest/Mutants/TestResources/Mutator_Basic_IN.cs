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

            var alt1 = array.Count(x => x % 2 == 0);
            var alt2 = array.Min();
        }
    }
}
