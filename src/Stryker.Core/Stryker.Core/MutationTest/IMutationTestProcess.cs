namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate();
        void Test(int maxConcurrentTestRunners);
    }
}