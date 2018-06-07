using Stryker.Core.Mutants;

namespace Stryker.Core.MutationTest
{
    /// <summary>
    /// Executes exactly one mutationtest and stores the result
    /// </summary>
    public interface IMutationTestExecutor
    {
        void Test(Mutant mutant);
    }
}
